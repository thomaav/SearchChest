using Microsoft.Xna.Framework.Input;

namespace SearchChests
{
    internal class ModConfig
    {
        public Keys SearchKey { get; set; } = Keys.S;
        public bool UseControlModifier { get; set; } = true;
    }
}
