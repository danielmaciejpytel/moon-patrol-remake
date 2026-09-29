using UnityEngine;

namespace UI.Kit {
    /// <summary>Paleta interfejsu z makiety: kosmos, szary regolit, turkus miasta obcych, pomarańcz UFO.</summary>
    public static class UiColors {
        public static readonly Color Void = Hex(0x0b0d10);
        public static readonly Color Panel = Hex(0x14181c);
        public static readonly Color Line = Hex(0x2a3137);
        public static readonly Color Regolith = Hex(0xb9bec3);
        public static readonly Color Ink = Hex(0xe8ecef);
        public static readonly Color Dim = Hex(0x8b959d);
        public static readonly Color Teal = Hex(0x1f8a80);
        public static readonly Color TealHi = Hex(0x43d1c0);
        public static readonly Color Ufo = Hex(0xf0a13a);
        public static readonly Color Signal = Hex(0xe8265f);
        public static readonly Color Good = Hex(0x58c98a);

        public static Color Hex(int rgb, float alpha = 1f) =>
            new Color(((rgb >> 16) & 0xff) / 255f, ((rgb >> 8) & 0xff) / 255f, (rgb & 0xff) / 255f, alpha);

        public static Color WithAlpha(this Color c, float a) => new Color(c.r, c.g, c.b, a);
    }
}
