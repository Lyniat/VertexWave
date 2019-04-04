using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TrueCraft.Client.Rendering
{
    /// <summary>
    ///     Represents a font.
    /// </summary>
    public class Font
    {
        private FontFile _definition;
        private Dictionary<char, FontChar> _glyphs;
        private Texture2D[] _textures;

        /// <summary>
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="name"></param>
        /// <param name="style"></param>
        public Font(ContentManager contentManager, string name, FontStyle style = FontStyle.Regular)
        {
            Name = name;
            Style = style;

            LoadContent(contentManager);
            GenerateGlyphs();
        }

        /// <summary>
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// </summary>
        public FontStyle Style { get; }

        /// <summary>
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public Texture2D GetTexture(int page = 0)
        {
            return _textures[page];
        }

        /// <summary>
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public FontChar GetGlyph(char ch)
        {
            FontChar glyph = null;
            _glyphs.TryGetValue(ch, out glyph);
            return glyph;
        }

        /// <summary>
        /// </summary>
        /// <param name="contentManager"></param>
        private void LoadContent(ContentManager contentManager)
        {
            var definitionPath = "Content/" + string.Format("{0}_{1}.fnt", Name, Style);
            //File.ReadAllText(Configuration.Path + "conf.json");
            using (var contents = new StreamReader(definitionPath).BaseStream)
                //using (var contents = File.OpenRead(Path.Combine(contentManager.RootDirectory, definitionPath)))
            {
                _definition = FontLoader.Load(contents);
            }

            if (_textures != null)
                for (var i = 0; i < _textures.Length; i++)
                    _textures[i].Dispose();

            // We need to support multiple texture pages for more than plain ASCII text.
            _textures = new Texture2D[_definition.Pages.Count];
            for (var i = 0; i < _definition.Pages.Count; i++)
            {
                var texturePath = "Content/" + string.Format("{0}_{1}_{2}.png", Name, Style, i);
                var filestream = new FileStream(texturePath, FileMode.Open);
                _textures[i] = Texture2D.FromStream(VoxeLand.game.GraphicsDevice, filestream);
            }
        }

        /// <summary>
        /// </summary>
        private void GenerateGlyphs()
        {
            _glyphs = new Dictionary<char, FontChar>();
            foreach (var glyph in _definition.Chars)
            {
                var c = (char) glyph.Id;
                _glyphs.Add(c, glyph);
            }
        }
    }
}