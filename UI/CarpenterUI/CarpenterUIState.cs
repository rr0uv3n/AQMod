﻿using Aequus.Content.CarpenterBounties;
using Aequus.Graphics.RenderTargets;
using Aequus.Items.Misc;
using Aequus.Items.Tools.Camera;
using Aequus.NPCs.Friendly.Town;
using Aequus.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.UI.CarpenterUI
{
    public class CarpenterUIState : AequusUIState
    {
        public UIList bountyList;
        public UIScrollbar bountyListScrollBar;

        public UIPanel selectionPanel;
        public UIList selectionPanelList;
        public UIScrollbar selectionPanelListScrollBar;
        public ItemSlotElement submissionSlot;
        public UIText submissionSlotTextButton;
        public UIImageButton backButton;
        public CarpenterUIElement selected;

        public string SubmitPhotoText => AequusText.GetText("Chat.Carpenter.UI.SubmitPhoto");

        public override void OnInitialize()
        {
            OverrideSamplerState = SamplerState.LinearClamp;

            Height.Set(0, 0.526f);
            MinWidth.Set(400, 0f);
            MaxWidth.Set(1000, 0f);
            MinHeight.Set(400, 0f);
            MaxHeight.Set(1000, 0f);
            Top.Set(100, 0f);
            HAlign = 0.5f;

            SetListPanel();
        }

        public override void OnDeactivate()
        {
            Clear();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Main.playerInventory = !Main.mouseItem.IsAir || selected != null;
            if (NotTalkingTo<Carpenter>())
            {
                Aequus.NPCTalkInterface.SetState(null);
                return;
            }

            if (selected != null)
            {
                if (selectionPanel == null)
                    SetViewBountyPanel();
            }
            else
            {
                if (bountyList == null)
                    SetListPanel();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var d = GetDimensions();
            var rect = d.ToRectangle();
            if (rect.Contains(Main.mouseX, Main.mouseY))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            Utils.DrawInvBG(spriteBatch, rect, (AequusUI.invBackColor * 0.5f).UseA(255) * AequusUI.invBackColorMultipler);

            if (submissionSlot != null && submissionSlot.IsMouseHovering && submissionSlot.HasItem)
            {
                AequusUI.HoverItem(submissionSlot.item);
            }

            if (submissionSlotTextButton != null)
            {
                if (submissionSlotTextButton.IsMouseHovering && submissionSlot.HasItem)
                {
                    string t = SubmitPhotoText;
                    if (submissionSlotTextButton.Text == t)
                    {
                        submissionSlotTextButton.SetText(AequusText.ColorCommand(t, Color.Yellow));
                    }
                    if (Main.mouseLeft && Main.mouseLeftRelease && submissionSlot.item.ModItem is ShutterstockerClip clip)
                    {
                        Main.mouseLeftRelease = false;

                        bool completed = selected.bounty.CheckConditions(new CarpenterBounty.ConditionInfo(clip, Main.npc[Main.LocalPlayer.talkNPC]), out string responseMessage);
                        ShutterstockerSceneRenderer.renderRequests.Add(clip);
                        clip.reviewed = true;
                        SoundEngine.PlaySound(SoundID.Chat);
                        Main.playerInventory = true;
                        Main.npcChatText = responseMessage;
                        Aequus.NPCTalkInterface.SetState(null);
                        if (completed)
                        {
                            Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>().CompletedBounties.Add(selected.bounty.FullName);
                            selected.bounty.OnCompleteBounty(Main.LocalPlayer, Main.npc[Main.LocalPlayer.talkNPC]);
                        }
                    }
                }
                else
                {
                    string t = SubmitPhotoText;
                    if (submissionSlotTextButton.Text != t)
                    {
                        submissionSlotTextButton.SetText(t);
                    }
                }
            }

            if (bountyList != null)
            {
                var listBox = bountyList.GetDimensions().ToRectangle();
                var listBoxDraw = listBox;
                bountyListScrollBar.Height.Set(listBox.Height - 8 - bountyListScrollBar.Top.Pixels, 0f);
                listBoxDraw.Y -= 4;
                listBoxDraw.Height += 8;
                Utils.DrawInvBG(spriteBatch, listBoxDraw);
            }
            if (selectionPanelList != null)
            {
                ManageSelectionPanelScrollbarHeight();
            }
            base.DrawSelf(spriteBatch);
        }

        private void ManageSelectionPanelScrollbarHeight()
        {
            var listBox = selectionPanelList.GetDimensions().ToRectangle();
            var listBoxDraw = listBox;
            selectionPanelListScrollBar.Height.Set(listBox.Height - 8 - selectionPanelListScrollBar.Top.Pixels, 0f);
            listBoxDraw.Y -= 4;
            listBoxDraw.Height += 8;
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }

        public void Select(CarpenterUIElement element)
        {
            if (element == selected)
                selected = null;
            selected = element;
        }

        public void SetListPanel()
        {
            Clear();
            selected = null;

            Width.Set(128, 0.4f);

            bountyList = new UIList();
            bountyList.Left.Set(20, 0f);
            bountyList.Top.Set(20, 0f);
            bountyList.Width.Set(-20, 1f);
            bountyList.Height.Set(-40, 1f);

            bountyListScrollBar = new UIScrollbar();
            bountyListScrollBar.Left.Set(-28, 1f);
            bountyListScrollBar.Top.Set(8, 0f);
            bountyListScrollBar.Height.Set(400, 0f);
            bountyList.SetScrollbar(bountyListScrollBar);
            bountyList.Append(bountyListScrollBar);

            Append(bountyList);
            Recalculate();

            var bountyPlayer = Main.LocalPlayer.GetModPlayer<CarpenterBountyPlayer>();
            foreach (var bounty in CarpenterSystem.BountiesByID)
            {
                if (!bounty.IsBountyAvailable() || bountyPlayer.CompletedBounties.Contains(bounty.FullName))
                    continue;

                var uiElement = new CarpenterUIElement(this, bounty);
                uiElement.Width.Set(-48, 1f);
                uiElement.Height.Set(160, 0f);
                uiElement.Left.Set(10, 0f);
                bountyList.Add(uiElement);
                uiElement.Setup();
            }

            return;
        }

        public void SetViewBountyPanel()
        {
            Width = new StyleDimension(80, 0.35f);

            Clear();

            selectionPanel = new UIPanel();
            selectionPanel.Left.Set(20, 0f);
            selectionPanel.Top.Set(20, 0f);
            selectionPanel.Width.Set(-40, 1f);
            selectionPanel.Height.Set(-40, 1f);
            selectionPanel.BorderColor = selectionPanel.BackgroundColor * 2f;
            float colorMult = 1f / (selectionPanel.BackgroundColor.A / 255f);
            selectionPanel.BackgroundColor *= colorMult;

            Append(selectionPanel);

            selectionPanelList = new UIList();
            selectionPanelList.Left.Set(32, 0f);
            selectionPanelList.Top.Set(0, 0f);
            selectionPanelList.Width.Set(-selectionPanelList.Left.Pixels * 2, 1f);
            selectionPanelList.Height.Set(0, 1f);

            selectionPanelListScrollBar = new UIScrollbar();
            selectionPanelListScrollBar.Left.Set(-12, 1f);
            selectionPanelListScrollBar.Top.Set(8, 0f);
            selectionPanelListScrollBar.Width.Set(32, 0f);
            selectionPanelListScrollBar.Height.Set(460, 0f);
            selectionPanelList.SetScrollbar(selectionPanelListScrollBar);
            selectionPanel.Append(selectionPanelList);
            selectionPanel.Append(selectionPanelListScrollBar);

            backButton = new UIImageButton(ModContent.Request<Texture2D>(Aequus.VanillaTexture + "UI/Bestiary/Button_Back", AssetRequestMode.ImmediateLoad))
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(28, 0f),
                Height = new StyleDimension(28, 0f),
            };
            backButton.OnClick += BackButton_OnClick;

            selectionPanel.Append(backButton);
            PopulateSelectPanelList(selected);
        }

        private void BackButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            selected = null;
        }

        public void Clear()
        {
            if (submissionSlot != null && submissionSlot.HasItem)
            {
                Main.LocalPlayer.QuickSpawnClonedItem(null, submissionSlot.item, submissionSlot.item.stack);
            }

            bountyList = null;
            bountyListScrollBar = null;
            selectionPanel = null;
            selectionPanelList = null;
            selectionPanelListScrollBar = null;
            backButton = null;
            submissionSlot = null;
            RemoveAllChildren();
        }

        public void PopulateSelectPanelList(CarpenterUIElement element)
        {
            TitleAndDescription(element);
            Requirements(element);

            AddSeparator();

            Submission(element);

            AddSeparator();

            Blueprint(element);
        }
        public void TitleAndDescription(CarpenterUIElement element)
        {
            var panel = new UIPanel()
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(0, 1f),
                Height = new StyleDimension(56, 0f),
                BackgroundColor = (AequusUI.invBackColor * 0.85f).UseA(255),
                BorderColor = Color.Transparent,
                PaddingLeft = 0f,
                PaddingTop = 0f,
            };
            panel.Append(new UIText(element.listItem.ModItem<CarpenterBountyItem>().BountyName)
            {
                Width = panel.Width,
                Height = panel.Height,
                TextOriginX = 0.5f,
                TextOriginY = 0.25f,
                TextColor = Color.Lerp(Color.Yellow, Color.White, 0.45f),
            });
            panel.Append(new UIText(element.listItem.ModItem<CarpenterBountyItem>().BountyDescription, textScale: 0.85f)
            {
                Width = panel.Width,
                Height = panel.Height,
                Top = new StyleDimension(24, 0f),
                TextOriginX = 0.5f,
                TextOriginY = 0.25f,
            });
            selectionPanelList.Add(panel);
        }
        public void Requirements(CarpenterUIElement element)
        {
            var panel = new UIPanel()
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(0, 1f),
                Height = new StyleDimension(1000, 0f),
                BackgroundColor = (AequusUI.invBackColor * 0.6f).UseA(255),
                BorderColor = Color.Transparent,
                PaddingLeft = 0f,
                PaddingTop = 0f,
            };
            selectionPanelList.Add(panel);

            string requirementText = element.listItem.ModItem<CarpenterBountyItem>().BountyRequirements;
            var split = requirementText.Split('\n');
            requirementText = "";
            foreach (string s in split)
            {
                if (requirementText != "")
                    requirementText += "\n";
                requirementText += "• " + FontAssets.MouseText.Value.CreateWrappedText(s, panel.GetInnerDimensions().Width);
            }
            var uiText = new UIText(requirementText, textScale: 0.8f)
            {
                Width = panel.Width,
                Height = panel.Height,
                Top = new StyleDimension(10, 0f),
                TextOriginX = 0f,
                TextOriginY = 0f,
                PaddingLeft = 4f,
            };
            panel.Append(uiText);
            panel.Height.Set((int)FontAssets.MouseText.Value.MeasureString(uiText.Text).Y * 0.8f + uiText.Top.Pixels, 0f);
            uiText.IsWrapped = true;
        }
        public void Submission(CarpenterUIElement element)
        {
            var panel = new UIPanel()
            {
                Top = new StyleDimension(0, 0f),
                Left = new StyleDimension(0, 0f),
                Width = new StyleDimension(120, 0f),
                Height = new StyleDimension(1000, 0f),
                BackgroundColor = (AequusUI.invBackColor * 0.6f).UseA(255),
                BorderColor = Color.Transparent,
                HAlign = 0.5f,
            };
            selectionPanelList.Add(panel);

            submissionSlot = new ItemSlotElement(TextureAssets.InventoryBack.Value)
            {
                Top = new StyleDimension(0, 0f),
                HAlign = 0.5f,
                Width = new StyleDimension(64 * 0.8f, 0f),
                Height = new StyleDimension(64 * 0.8f, 0f),
            };
            submissionSlot.OnClick += SubmissionSlot_OnClick;
            panel.Append(submissionSlot);

            var text = SubmitPhotoText;
            submissionSlotTextButton = new UIText(text, 1f)
            {
                Width = panel.Width,
                Height = panel.Height,
                Top = new StyleDimension(submissionSlot.Height.Pixels + 8, 0f),
                TextOriginX = 0.5f,
                TextOriginY = 0f,
                HAlign = 0.5f,
                IgnoresMouseInteraction = false,
            };
            panel.Append(submissionSlotTextButton);
            var textMeasurement = FontAssets.MouseText.Value.MeasureString(text);

            panel.Width.Set(Math.Max((int)submissionSlot.Width.Pixels, (int)textMeasurement.X) + 20, 0f);
            panel.Height.Set((int)textMeasurement.Y + submissionSlot.Height.Pixels + 16, 0f);
        }

        private void SubmissionSlot_OnClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (Main.mouseItem.IsAir)
            {
                if (submissionSlot.HasItem)
                {
                    Utils.Swap(ref Main.mouseItem, ref submissionSlot.item);
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                return;
            }

            if (Main.mouseItem.ModItem is ShutterstockerClip clip)
            {
                Utils.Swap(ref Main.mouseItem, ref submissionSlot.item);
                SoundEngine.PlaySound(SoundID.Grab);
            }
        }

        public void Blueprint(CarpenterUIElement element)
        {
            string texture = element.ListItem.BountyTexture;
            if (!ModContent.HasAsset(texture))
            {
                return;
            }

            var panel = new UIPanel()
            {
                Width = new StyleDimension(0, 0.9f),
                BackgroundColor = (AequusUI.invBackColor * 0.6f).UseA(255),
                BorderColor = Color.Transparent,
                PaddingLeft = 0f,
                PaddingTop = 0f,
                HAlign = 0.5f,
            };

            panel.Append(new UIText(AequusText.GetText("Chat.Carpenter.UI.Blueprint"), 0.85f)
            {
                Width = panel.Width,
                Height = panel.Height,
                TextOriginX = 0f,
                PaddingLeft = 6f,
                PaddingTop = 10f,
            });

            panel.Append(new UIText(AequusText.GetText("Chat.Carpenter.UI.BlueprintFakeCopyright"), 0.6f)
            {
                Width = panel.Width,
                Height = panel.Height,
                TextOriginX = 0f,
                PaddingLeft = 6f,
                PaddingTop = 30f,
            });

            selectionPanelList.Add(panel);

            var textureElement = new CarpenterTextureUIElement(ModContent.Request<Texture2D>(texture, AssetRequestMode.ImmediateLoad))
            {
                Width = new StyleDimension(0, 1f),
                Top = new StyleDimension(44, 0f),
            };
            textureElement.OnRecalculateTextureSize += () => panel.Height = new StyleDimension(textureElement.Height.Pixels + textureElement.Top.Pixels, 0f);
            panel.Append(textureElement);

            AddSeparator();
        }

        public void AddSeparator()
        {
            selectionPanelList.Add(new UIHorizontalSeparator()
            {
                Left = new StyleDimension(12f, 0f),
                Width = new StyleDimension(-24f, 1f),
                Color = AequusUI.invBackColor * 2f * AequusUI.invBackColorMultipler,
            });
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