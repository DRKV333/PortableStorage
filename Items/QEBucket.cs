using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PortableStorage.TileEntities;
using ReLogic.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TheOneLibrary.Fluid;
using static TheOneLibrary.Utility.Utility;

namespace PortableStorage.Items
{
	public class QEBucket : BaseBag
	{
		public Frequency frequency;

		public override string Texture => PortableStorage.ItemTexturePath + "QEBucket";

		public override ModItem Clone(Item item)
		{
			QEBucket clone = (QEBucket)base.Clone(item);
			clone.frequency = frequency;
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quantum Entangled Bucket");
			Tooltip.SetDefault("Right-click on a Quantum Entangled Tank to link it");
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 34;
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 15;
			item.maxStack = 1;
			item.autoReuse = true;
			item.value = GetItemValue(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>()) + GetItemValue(ItemID.ShadowScale) * 25 + GetItemValue(ItemID.DemoniteBar) * 5;
			item.rare = 9;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool UseItem(Player player)
		{
			ModFluid fluid = mod.GetModWorld<PSWorld>().enderFluids[frequency];
			if (player.altFunctionUse == 2)
			{
				if (fluid != null)
				{
					Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
					if ((!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]) && TileLoader.GetTile(tile.type)?.GetType().GetAttribute<BucketDisablePlacement>() == null)
					{
						if (tile.liquid == 0 || tile.liquidType() == fluid.type)
						{
							Main.PlaySound(19, (int)player.position.X, (int)player.position.Y);

							if (tile.liquid == 0) tile.liquidType(fluid.type);

							int volume = Math.Min(fluid.volume, 255 - tile.liquid);
							tile.liquid += (byte)volume;
							fluid.volume -= volume;
							if (fluid.volume == 0) fluid = null;

							WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY);

							if (Main.netMode == 1) NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
						}
					}
				}
			}
			else
			{
				if (!Main.GamepadDisableCursorItemIcon)
				{
					player.showItemIcon = true;
					Main.ItemIconCacheUpdate(item.type);
				}

				Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
				if ((fluid == null || fluid.type == tile.liquidType()) && tile.liquid > 0 && TileLoader.GetTile(tile.type)?.GetType().GetAttribute<BucketDisablePickup>() == null)
				{
					Main.PlaySound(19, (int)player.position.X, (int)player.position.Y);

					if (fluid == null) fluid = TheOneLibrary.Utility.Utility.SetDefaults(tile.liquidType());

					int drain = Math.Min(tile.liquid, TEQETank.MaxVolume - fluid.volume);
					fluid.volume += drain;

					tile.liquid -= (byte)drain;

					if (tile.liquid <= 0)
					{
						tile.lava(false);
						tile.honey(false);
					}

					WorldGen.SquareTileFrame(Player.tileTargetX, Player.tileTargetY, false);
					if (Main.netMode == 1) NetMessage.sendWater(Player.tileTargetX, Player.tileTargetY);
					else Liquid.AddWater(Player.tileTargetX, Player.tileTargetY);
				}
			}

			mod.GetModWorld<PSWorld>().enderFluids[frequency] = fluid;

			return true;
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.Draw(PortableStorage.ringBig, position + new Vector2(4, 16) * scale, new Rectangle(0, 4 * (int)frequency.colorLeft, 22, 4), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.ringBig, position + new Vector2(4, 20) * scale, new Rectangle(0, 4 * (int)frequency.colorMiddle, 22, 4), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(PortableStorage.ringSmall, position + new Vector2(6, 24) * scale, new Rectangle(0, 4 * (int)frequency.colorRight, 18, 4), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			//tooltips.Add(new TooltipLine(mod, "BagInfo", $"Use the bag, right-click it or press [c/83fcec:{GetHotkeyValue(mod.Name + ": Open Bag")}] while having it in an accessory slot to open it"));
		}

		public override TagCompound Save() => new TagCompound {["Frequency"] = frequency};

		public override void Load(TagCompound tag)
		{
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer) => TagIO.Write(Save(), writer);

		public override void NetRecieve(BinaryReader reader) => Load(TagIO.Read(reader));

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>());
			recipe.AddIngredient(ItemID.ShadowScale, 25);
			recipe.AddIngredient(ItemID.DemoniteBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
			recipe = new ModRecipe(mod);
			recipe.AddIngredient(TheOneLibrary.TheOneLibrary.Instance.ItemType<Bucket>());
			recipe.AddIngredient(ItemID.TissueSample, 25);
			recipe.AddIngredient(ItemID.CrimtaneBar, 5);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}