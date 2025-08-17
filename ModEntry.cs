using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Threading.Tasks;

namespace MobileSafeQuickSave_API
{
    public sealed class ModEntry : Mod
    {
        private bool _hasSpaceCore;
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            _hasSpaceCore = helper.ModRegistry.IsLoaded("spacechase0.SpaceCore");
            Monitor.Log($"SpaceCore detected: {_hasSpaceCore}", LogLevel.Info);

            // Config null 체크 및 예외 처리
            try
            {
                Config = helper.ReadConfig<ModConfig>() ?? new ModConfig();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Config 읽기 실패: {ex}", LogLevel.Warn);
                Config = new ModConfig();
            }

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.ConsoleCommands.Add("msqsave", "보수적 중간저장 시도", (_, __) => _ = TrySaveAsync(false));
            helper.ConsoleCommands.Add("msqsave!", "강제 중간저장(비추천)", (_, __) => _ = TrySaveAsync(true));
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && e.Button == SButton.F5)
                _ = TrySaveAsync(false);
        }

        private async Task TrySaveAsync(bool force)
        {
            try
            {
                if (!Context.IsWorldReady)
                {
                    Monitor.Log("월드 준비 전", LogLevel.Warn);
                    return;
                }

                if (!force && !IsSafeToSave(out string whyNot))
                {
                    Monitor.Log($"저장 거부: {whyNot}", LogLevel.Warn);
                    return;
                }

                await Task.Delay(Config.ExtraDelayMs);

                if (!force && !IsSafeToSave(out string recheck))
                {
                    Monitor.Log($"저장 직전 재검사 실패: {recheck}", LogLevel.Warn);
                    return;
                }

                Monitor.Log("중간 저장 시작...", LogLevel.Info);
                Game1.saveOnNewDay = false;
                Game1.SaveGame();
                Monitor.Log("중간 저장 완료", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"저장 중 예외 발생: {ex}", LogLevel.Error);
            }
        }

        private bool IsSafeToSave(out string reason)
        {
            if (!Context.IsWorldReady) { reason = "월드 준비 전"; return false; }
            if (Game1.IsSaving) { reason = "이미 저장 중"; return false; }
            if (Game1.newDay || Game1.saveOnNewDay) { reason = "하루 전환 저장과 겹침"; return false; }
            if (Game1.eventUp || (Game1.currentLocation != null && Game1.currentLocation.currentEvent != null)) { reason = "이벤트/축제 중"; return false; }
            if (Game1.activeClickableMenu != null && !(Game1.activeClickableMenu is GameMenu)) { reason = "특수 메뉴/미니게임 중"; return false; }
            if (Game1.player == null || !Game1.player.CanMove) { reason = "플레이어 조작 불가"; return false; }

            // SpaceCore 설치 시 세이브/직렬화 체크
            if (_hasSpaceCore && SaveGame.IsProcessing)
            {
                reason = "세이브/직렬화 처리 중";
                return false;
            }

            reason = "";
            return true;
        }
    }

    public class ModConfig
    {
        public int ExtraDelayMs { get; set; } = 750;
        public bool VerboseLogging { get; set; } = true;
    }
}