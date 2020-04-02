using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

using CoreModule.Drawables;
using CoreModule.Terrain;
using CoreModule.Shapes;
using Point = CoreModule.Shapes.Point;

namespace CoreModule.Scenes {
    public class TileEditor : Scene {
        ColourPicker picker;
        TextBox name;
        NumberBox id;
        CheckBox collide;
        Button save;
        Button load;
        Rect editingArea;
        int scale;
        Sprite sprite;

        //Dictionary<>

        public TileEditor() {
            sprite = new Sprite(Tile.TileSize, Tile.TileSize);

            scale = 200 / Tile.TileSize;
            editingArea = new Rect(new Point(0, 0), scale * Tile.TileSize, scale * Tile.TileSize);

            picker = new ColourPicker(new Point(editingArea.Right + 20, 10));
            Drawables.Add(picker);

            name = new TextBox("name", picker.Bounds.Center.X, picker.Bounds.Bottom + 10);
            save = new Button("Save", name.Bounds.Center.X, name.Bounds.Bottom + 10);
            load = new Button("Load", name.Bounds.Center.X, save.Bounds.Bottom + 10);

            id = new NumberBox(0, name.Bounds.Center.X, load.Bounds.Bottom + 10, 0, 255);
            collide = new CheckBox(id.Bounds.Right + 10, id.Bounds.Center.Y);

            save.Pressed += SaveTile;
            load.Pressed += LoadTile;
            Drawables.Add(name);
            Drawables.Add(save);
            Drawables.Add(load);
            Drawables.Add(id);
            Drawables.Add(collide);

            RefreshTileManager();
        }


        void RefreshTileManager() {
            TileManager.Graphics.Clear();
            TileManager.Collideable.Clear();
            TileManager.Names.Clear();
            TileManager.IDs.Clear();

            TileManager.Setup();
        } 
        private void LoadTile(Button sender) {
            RefreshTileManager();
            byte id = TileManager.GetTypeFromName(this.name.Text);
            bool collide = TileManager.IsSolid(id);
            string name = TileManager.GetName(id);

            this.id.Value = id;
            this.collide.On = collide;
            this.name.Text = name;

            sprite = TileManager.GetTexture(id);
        }
        private void SaveTile(Button sender) {
            Sprite.Save(sprite, $"Assets/Terrain/{name.Text}.png");
            TileManager.AddToManifest((byte)id.Value, name.Text, collide.On);
            TileManager.SaveManifest();
            RefreshTileManager();
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            if (Collision.WithinRect(editingArea, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY))) {
                int editX = (CoreGame.Instance.MouseX) / scale;
                int editY = (CoreGame.Instance.MouseY) / scale;

                if (CoreGame.Instance.GetMouse(Mouse.Left).Down) sprite[editX, editY] = picker.Value;
                if (CoreGame.Instance.GetMouse(Mouse.Right).Down) sprite[editX, editY] = Pixel.Empty;
                if (CoreGame.Instance.GetMouse(Mouse.Middle).Down) picker.Value = sprite[editX, editY];
            }

            if (CoreGame.Instance.GetKey(Key.Escape).Pressed) CoreGame.Instance.PopScene();
        } 

        public override void Draw() {
            base.Draw();

            for (int x = 0; x < sprite.Width; x++) {
                for (int y = 0; y < sprite.Height; y++) {
                    CoreGame.Instance.FillRect(new Point(x * scale, y * scale) + editingArea.TopLeft, scale, scale, sprite[x, y]);
                }
            }

            if (Collision.WithinRect(editingArea, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY))) {
                int editX = (CoreGame.Instance.MouseX) / scale;
                int editY = (CoreGame.Instance.MouseY) / scale;

                CoreGame.Instance.FillRect(new Point(editX * scale, editY * scale), scale, scale, picker.Value);
            }

            CoreGame.Instance.DrawSprite(new Point(0, 0), sprite);
            CoreGame.Instance.DrawRect(editingArea.TopLeft, editingArea.BottomRight, Pixel.Presets.White);
        }
    }
}
