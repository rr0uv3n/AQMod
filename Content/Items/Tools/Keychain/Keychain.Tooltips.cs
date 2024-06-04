﻿using Aequus.Common.Items.Components;
using Aequus.Common.Items.Tooltips;
using System.Collections.Generic;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.GameInput;
using Terraria.Localization;

namespace Aequus.Content.Items.Tools.Keychain;

public partial class Keychain : IAddKeywords {
    public void AddSpecialTooltips() {
        KeychainPlayer keychain = Main.LocalPlayer.GetModPlayer<KeychainPlayer>();
        if (keychain.sortedKeysForIcons == null || keychain.sortedKeysForIcons.Count == 0) {
            return;
        }

        Keyword keyword = new Keyword(Language.GetTextValue("Mods.Aequus.Misc.ContainsHeader"), Color.Gold, ItemID.GoldenKey);
        foreach (Item item in keychain.sortedKeysForIcons) {
            int stack = item.stack;
            item.stack = 1;
            Color color = item.rare == ItemRarityID.White ? Color.White : ItemRarity.GetColor(item.rare);
            string text = $"{ItemTagHandler.GenerateTag(item)}{ChatTagWriter.Color(color, item.Name)}";
            item.stack = stack;

            if (item.stack > 1) {
                text += $" ({item.stack})";
            }

            keyword.AddLine(text);
        }

        KeywordSystem.Tooltips.Add(keyword);
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips) {
        string smartSelectButton = PlayerInput.GenerateInputTag_ForCurrentGamemode_WithHacks(tagForGameplay: false, "SmartSelect");
        if (!smartSelectButton.Contains('[')) {
            smartSelectButton = ExtendLanguage.PrettyPrint(smartSelectButton);
        }

        foreach (TooltipLine line in tooltips) {
            if (line.Name.StartsWith("Tooltip")) {
                line.Text = line.Text.Replace("<shift>", smartSelectButton);
            }
        }
    }
}