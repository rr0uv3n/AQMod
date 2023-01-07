﻿using Aequus.Content.Carpentery.Bounties;
using Aequus.NPCs.Friendly.Town;
using Aequus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace Aequus.Content.Carpentery.Bounties.BountyUI
{
    public class BountyUIState : AequusUIState
    {
        public BountyDetailsPanelManager detailsManager;

        public override void OnInitialize()
        {
            OverrideSamplerState = SamplerState.LinearClamp;

            MinWidth.Set(500, 0f);
            MinHeight.Set(500, 0f);
            Width.Set(250, 0.275f);
            Height.Set(0, 0.75f);
            Top.Set(100, 0f);
            HAlign = 0.5f;

            var uiPanel = new UIPanel();
            uiPanel.BackgroundColor = new Color(68, 99, 164) * 0.5f;
            uiPanel.Width.Set(0, 1f);
            uiPanel.Height.Set(0, 1f);
            Append(uiPanel);

            SetupBountySidebar();
            SetupDetailsPanel();
        }

        public void SetupDetailsPanel()
        {
            var uiPanel = new UIPanel();
            uiPanel.BackgroundColor = new Color(34, 64, 126) * 0.75f;
            uiPanel.Top.Set(10, 0f);
            uiPanel.Left.Set(20 + 220, 0f);
            uiPanel.Width.Set(-220 - 30, 1f);
            uiPanel.Height.Set(-20, 1f);
            Append(uiPanel);
            detailsManager = new BountyDetailsPanelManager(this, uiPanel);
            Append(detailsManager);
        }

        public void SetupBountySidebar()
        {
            var uiPanel = new UIPanel();
            uiPanel.BackgroundColor = new Color(91, 124, 193) * 0.9f;
            uiPanel.BorderColor = uiPanel.BackgroundColor * 0.5f;
            uiPanel.Top.Set(10, 0f);
            uiPanel.Left.Set(10, 0f);
            uiPanel.Width.Set(220, 0f);
            uiPanel.Height.Set(-20, 1f);
            Append(uiPanel);

            var uiList = new UIList();
            uiList.Width.Set(0, 1f);
            uiList.Height.Set(0, 1f);
            uiPanel.Append(uiList);

            var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
            foreach (var bounty in CarpenterSystem.BountiesByID)
            {
                if (!bounty.IsBountyAvailable() || CarpenterSystem.CompletedBounties.Contains(bounty.FullName))
                    continue;

                var uiElement = new BountySidebarElement(this, bounty);
                uiList.Add(uiElement);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (NotTalkingTo<Carpenter>())
            {
                Aequus.UserInterface.SetState(null);
                return;
            }
            if (GetDimensions().ToRectangle().Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            base.Update(gameTime);
        }

        public override void ConsumePlayerControls(Player player)
        {
            if (player.controlInv)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                player.releaseInventory = false;
                player.SetTalkNPC(-1);
                Aequus.UserInterface.SetState(null);
            }
        }

        public override int GetLayerIndex(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex((l) => l.Name.Equals(AequusUI.InterfaceLayers.Inventory_28));
            if (index == -1)
                return -1;
            return index + 1;
        }

        public override bool ModifyInterfaceLayers(List<GameInterfaceLayer> layers, ref InterfaceScaleType scaleType)
        {
            DisableAnnoyingInventoryLayeringStuff(layers);
            return true;
        }
    }
}