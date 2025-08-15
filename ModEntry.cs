using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Reflection;
using System.Threading.Tasks;

// Quick Save API 인터페이스 가정
namespace QuickSaveAPI
{
    public interface IQuickSaveAPI
    {
        bool CanSaveNow();
        void RequestSave();
    }
}

namespace MobileSafeQuickSave_API
{
    public sealed class ModEntry : Mod
    {
        private IModHelper _helper;
        private QuickSaveAPI.IQuickSaveAPI? _qsApi = null;

        public override void Entry(IModHelper helper)
        {
            _helper = helper;
            _qsApi = helper.ModRegistry.GetApi<QuickSaveAPI.IQuickSaveAPI>("delixx.QuickSave");

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.ConsoleCommands.Add("msqsave", "모바일 보수적 Quick Save 시도", (_, __) => _ = TrySaveAsync(false));
            helper.ConsoleCommands.Add("msqsave!", "모바일 강제 Quick Save(권장X)", (_, __) => _ = TrySaveAsync(true));
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && e.Button == SButton.F5)
                _ = TrySaveAsync(false);
        }

        private async Task TrySaveAsync(bool force)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("월드 준비 전, 저장 건너뜀.", LogLevel.Warn);
                return;
            }

            if (!force && !IsSafeToSave(out string whyNot))
            {
                Monitor.Log($"저장 거부: {whyNot}", LogLevel.Warn);
                return;
            }

            try
            {
                await Task.Delay(600); // 다른 모드 안정화 대기
                if (!force && !IsSafeToSave(out string recheck))
                {
                    Monitor.Log($"저장 직전 재검사 실패: {recheck}", LogLevel.Warn);
                    return;
                }

                if (_qsApi != null && !_qsApi.CanSaveNow() && !force)
                {
                    Monitor.Log("Quick Save API 판단: 저장 불가 시점", LogLevel.Warn);
                    return;
                }

                Monitor.Log("중간 저장 시도...", LogLevel.Info);
                if (_qsApi != null)
                    _qsApi.RequestSave();
                else
                    Monitor.Log("QuickSave API를 찾을 수 없음. 저장이 권장되지 않아 수행하지 않습니다.", LogLevel.Warn);

                Monitor.Log("중간 저장 완료", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"저장 중 오류: {ex}", LogLevel.Error);
            }
        }

        private bool IsSafeToSave(out string reason)
        {
            if (Game1.IsSaving) { reason = "이미 저장 중"; return false; }
            if (Game1.eventUp || Game1.currentLocation?.currentEvent != null) { reason = "이벤트/축제 중"; return false; }
            if (Game1.fadeToBlack || Game1.panMode || Game1.warpingForForcedRemoteFarmerHazard) { reason = "화면 전환/워프 중"; return false; }
            if (Game1.activeClickableMenu is not null && Game1.activeClickableMenu.GetType() != typeof(GameMenu)) { reason = "메뉴/미니게임 중"; return false; }
            if (Game1.player is null || !Game1.player.CanMove) { reason = "플레이어 조작 불가"; return false; }
            if (Game1.newDay || Game1.saveOnNewDay) { reason = "하루 전환 세이브와 겹침"; return false; }
            if (!IsFtMIdle()) { reason = "Farm Type Manager 바쁨 가능성"; return false; }
            if (!IsSpaceCoreIdle()) { reason = "SpaceCore 직렬화 중 가능성"; return false; }
            reason = "";
            return true;
        }

        private bool IsFtMIdle()
        {
            if (!_helper.ModRegistry.IsLoaded("Esca.FarmTypeManager")) return true;
            try
            {
                var t = Type.GetType("FarmTypeManager.State");
                if (t != null)
                {
                    foreach (var fld in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                    {
                        var n = fld.Name.ToLowerInvariant();
                        if ((n.Contains("busy") || n.Contains("processing")) && fld.FieldType == typeof(bool))
                        {
                            if (fld.GetValue(null) is bool b) return !b;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"FTM 상태 검사 중 오류: {ex}", LogLevel.Warn);
            }
            return false;
        }

        private bool IsSpaceCoreIdle()
        {
            if (!_helper.ModRegistry.IsLoaded("spacechase0.SpaceCore")) return true;
            try
            {
                var t = Type.GetType("SpaceCore.SpaceCore");
                if (t != null)
                {
                    string[] names = { "IsSaving", "IsSerializing", "IsWorking" };
                    foreach (var name in names)
                    {
                        var prop = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (prop != null && prop.PropertyType == typeof(bool)) return !(bool)prop.GetValue(null, null)!;
                        var fld = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                        if (fld != null && fld.FieldType == typeof(bool)) return !(bool)fld.GetValue(null)!;
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"SpaceCore 상태 검사 중 오류: {ex}", LogLevel.Warn);
            }
            return !SaveGame.IsProcessing;
        }
    }
}
