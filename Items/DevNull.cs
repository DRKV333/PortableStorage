//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using PortableStorage.UI;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.ModLoader.IO;
//using Terraria.UI;
//using Terraria.UI.Chat;
//using TheOneLibrary.Base.UI;
//using TheOneLibrary.Storage;
//using static TheOneLibrary.Utility.Utility;

//namespace PortableStorage.Items
//{
//	public class DevNull : BaseBag, IContainerItem
//	{
//		public Guid guid = Guid.NewGuid();
//		public IList<Item> Items = new List<Item>();

//		public int selectedIndex = -1;

//		public override string Texture => PortableStorage.ItemTexturePath + "DevNull";

//		public override ModItem Clone(Item item)
//		{
//			DevNull clone = (DevNull)base.Clone(item);
//			clone.Items = Items;
//			clone.guid = guid;
//			return clone;
//		}

//		public override void SetStaticDefaults()
//		{
//			DisplayName.SetDefault("/dev/null");
//			//Tooltip.SetDefault("Restocks your ammo slots!");
//		}

//		public override void SetDefaults()
//		{
//			Items.Clear();
//			for (int i = 0; i < 27; i++) Items.Add(new Item());

//			item.width = 40;
//			item.height = 40;
//			item.useTime = 5;
//			item.useAnimation = 5;
//			item.useStyle = 1;
//			item.consumable = true;
//			//item.value = GetItemValue(ItemID.Leather) * 10;
//			item.rare = 1;
//		}

//		public override void HandleUI()
//		{
//			if (!PortableStorage.Instance.BagUI.ContainsKey(guid))
//			{
//				DevNullUI ui = new DevNullUI();
//				UserInterface userInterface = new UserInterface();
//				ui.Activate();
//				userInterface.SetState(ui);
//				ui.visible = true;
//				ui.Load(this);
//				PortableStorage.Instance.BagUI.Add(guid, new GUI(ui, userInterface));
//			}
//			else PortableStorage.Instance.BagUI.Remove(guid);

//			Main.PlaySound(SoundID.Item59);
//		}

//		public override bool CanUseItem(Player player) => selectedIndex >= 0 && Items[selectedIndex].stack > 1;

//		public override bool AltFunctionUse(Player player) => true;

//		public override bool ConsumeItem(Player player)
//		{
//			//Items[selectedIndex].stack--;
//			//Main.NewText();
//			return false;
//		}

//		public void SetItem(int index, bool wall = false)
//		{
//			selectedIndex = index;
//			if (index == -1)
//			{
//				item.placeStyle = -1;
//				item.createTile = -1;
//				item.createWall = 1;
//			}
//			else
//			{
//				item.placeStyle = Items[index].placeStyle;
//				if (wall) item.createWall = Items[index].createWall;
//				else item.createTile = Items[index].createTile;
//			}
//		}

//		public override bool UseItem(Player player)
//		{
//			//if (player.altFunctionUse == 2) HandleUI();
//			//else
//			//{
//			//	if (selectedIndex >= 0)
//			//	{
//			//		Item selectedItem = Items[selectedIndex];
//			//		if (selectedItem.createTile >= 0)
//			//		{
//			//			item.createTile = selectedItem.createTile;
//			//			item.placeStyle = selectedItem.placeStyle;
//			//			//Items[selectedIndex].stack--;
//			//		}
//			//		else if (selectedItem.createWall > 0)
//			//		{
//			//			item.createWall = selectedItem.createWall;
//			//			item.placeStyle = selectedItem.placeStyle;
//			//			//Items[selectedIndex].stack--;
//			//		}
//			//	}
//			//}

//			return true;
//		}

//		public override bool CanRightClick() => true;

//		public override void RightClick(Player player)
//		{
//			item.stack++;

//			HandleUI();
//		}

//		public override void ModifyTooltips(List<TooltipLine> tooltips)
//		{
//			tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
//		}

//		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
//		{
//			if (selectedIndex >= 0)
//			{
//				Item selectedItem = Items[selectedIndex];
//				Texture2D itemTexture = Main.itemTexture[selectedItem.type];
//				Rectangle rect = Main.itemAnimations[selectedItem.type] != null ? Main.itemAnimations[selectedItem.type].GetFrame(itemTexture) : itemTexture.Frame();
//				Color newColor = Color.White;
//				float pulseScale = 1f;
//				ItemSlot.GetItemLight(ref newColor, ref pulseScale, selectedItem);
//				int height = rect.Height;
//				int width = rect.Width;
//				float drawScale = 1f;
//				float availableWidth = 32;
//				if (width > availableWidth || height > availableWidth)
//				{
//					if (width > height) drawScale = availableWidth / width;
//					else drawScale = availableWidth / height;
//				}
//				drawScale *= scale;
//				Vector2 vector = new Vector2(40, 40) * scale;
//				Vector2 position2 = position + vector / 2f - rect.Size() * drawScale / 2f;
//				//Vector2 origin = rect.Size() * (pulseScale / 2f - 0.5f);

//				if (ItemLoader.PreDrawInInventory(selectedItem, spriteBatch, position2, rect, selectedItem.GetAlpha(newColor), selectedItem.GetColor(Color.White), origin, drawScale * pulseScale))
//				{
//					spriteBatch.Draw(itemTexture, position2, rect, selectedItem.GetAlpha(newColor), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
//					if (selectedItem.color != Color.Transparent) spriteBatch.Draw(itemTexture, position2, rect, selectedItem.GetColor(Color.White), 0f, origin, drawScale * pulseScale, SpriteEffects.None, 0f);
//				}
//				ItemLoader.PostDrawInInventory(selectedItem, spriteBatch, position2, rect, selectedItem.GetAlpha(newColor), selectedItem.GetColor(Color.White), origin, drawScale * pulseScale);
//				if (ItemID.Sets.TrapSigned[selectedItem.type]) spriteBatch.Draw(Main.wireTexture, position + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), Color.White, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);
//				if (selectedItem.stack > 1) ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, selectedItem.stack.ToSI("F1").TrimEnd('.', '0'), position + new Vector2(10f, 26f) * scale, Color.White, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
//				return false;
//			}

//			return true;
//		}

//		public override TagCompound Save() => new TagCompound { ["Items"] = Items.Save(), ["GUID"] = guid.ToString(), ["SelectedItem"] = selectedIndex };

//		public override void Load(TagCompound tag)
//		{
//			Items = TheOneLibrary.Utility.Utility.Load(tag);
//			guid = tag.ContainsKey("GUID") && !string.IsNullOrEmpty((string)tag["GUID"]) ? Guid.Parse(tag.GetString("GUID")) : Guid.NewGuid();
//			selectedIndex = tag.GetInt("SelectedItem");
//		}

//		public override void NetSend(BinaryWriter writer) => writer.Write(Items);

//		public override void NetRecieve(BinaryReader reader) => Items = TheOneLibrary.Utility.Utility.Read(reader);

//		public override void AddRecipes()
//		{
//			ModRecipe recipe = new ModRecipe(mod);
//			recipe.AddIngredient(ItemID.Leather, 10);
//			recipe.AddTile(TileID.WorkBenches);
//			recipe.SetResult(this);
//			recipe.AddRecipe();
//		}

//		public override void OnCraft(Recipe recipe)
//		{
//			Items.Clear();
//			for (int i = 0; i < 27; i++) Items.Add(new Item());
//		}

//		public IList<Item> GetItems() => Items;
//	}
//}