using System;
using System.Collections.Generic;
using Cairo;
using playerlist.util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public class GuiPlayerGrid : GuiElement {
    public const double Padding = 2;

    private readonly PlayerList _mod;
    private readonly List<IPlayer> _players;
    private readonly TextDrawUtil _textUtil;

    private readonly int _cols;
    private readonly int _rows;
    private readonly double _cellWidth;
    private readonly double _cellHeight;

    public GuiPlayerGrid(PlayerList mod, List<IPlayer> players, ElementBounds bounds) : base(mod.Api as ICoreClientAPI, bounds) {
        _mod = mod;
        _players = players;
        _textUtil = new TextDrawUtil();

        // only show the first 100 players
        int maxCount = Math.Min(players.Count, 100);
        // max 20 players per row
        _cols = (int)Math.Ceiling(maxCount / 20D); //Math.Min(5, (int)Math.Ceiling(Math.Sqrt(maxCount)));
        // evenly distribute players between columns
        _rows = (int)Math.Ceiling(maxCount / (double)_cols);

        // first thing's first. calculate max player name bounds
        ElementBounds maxBounds = new() {
            Alignment = EnumDialogArea.LeftTop,
            BothSizing = ElementSizing.Fixed
        };

        maxBounds.CalcWorldBounds();

        foreach (IPlayer player in players) {
            player.EntitlementColoredFont().AutoBoxSize(player.PlayerName ?? "", maxBounds, true);
        }

        _cellWidth = Math.Ceiling(maxBounds.fixedWidth) + 38;
        _cellHeight = Math.Max(25, Math.Ceiling(maxBounds.fixedHeight));

        Bounds.fixedWidth = (_cols * (Padding + _cellWidth)) - Padding;
        Bounds.fixedHeight = (_rows * (Padding + _cellHeight)) - Padding;
    }

    public override void ComposeElements(Context ctx, ImageSurface surface) {
        Bounds.CalcWorldBounds();
        double padding = scaled(Padding);
        double width = scaled(_cellWidth);
        double height = scaled(_cellHeight);
        int i = 0;
        for (int row = 0; row < _rows; ++row) {
            for (int col = 0; col < _cols; ++col) {
                double x = Bounds.drawX + (col * (width + padding));
                double y = Bounds.drawY + (row * (height + padding)) + padding;

                ctx.SetSourceRGBA(1.0, 1.0, 1.0, 0.2);
                Rectangle(ctx, x, y, width, height);
                ctx.Fill();

                if (i >= _players.Count) {
                    continue;
                }

                IPlayer player = _players[i++];
                surface.Image(_mod.PingIcon(player.Ping()), (int)(x + scaled(8)), (int)(y + scaled(4)), (int)scaled(16), (int)scaled(16));

                CairoFont font = player.EntitlementColoredFont();
                font.SetupContext(ctx);
                _textUtil.DrawTextLine(ctx, font, player.PlayerName, x + scaled(30), y);
            }
        }
    }
}
