"""Generate the Shiguang Clock application icon."""
from __future__ import annotations

import math
from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "assets"
SCALE = 4
SIZE = 256 * SCALE


def lerp(a: tuple[int, int, int], b: tuple[int, int, int], t: float) -> tuple[int, int, int]:
    return tuple(round(x + (y - x) * t) for x, y in zip(a, b))  # type: ignore[return-value]


def rounded_gradient(size: int) -> Image.Image:
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    grad = Image.new("RGBA", (size, size))
    px = grad.load()
    top = (25, 38, 61)
    bottom = (7, 12, 23)
    for y in range(size):
        c = lerp(top, bottom, y / max(1, size - 1))
        for x in range(size):
            px[x, y] = (*c, 255)
    mask = Image.new("L", (size, size), 0)
    ImageDraw.Draw(mask).rounded_rectangle(
        (0, 0, size - 1, size - 1), radius=int(size * 0.225), fill=255
    )
    img.alpha_composite(Image.composite(grad, Image.new("RGBA", (size, size)), mask))
    return img


def add_glow(img: Image.Image) -> None:
    glow = Image.new("RGBA", img.size, (0, 0, 0, 0))
    d = ImageDraw.Draw(glow)
    d.ellipse(
        (
            int(img.width * 0.15),
            int(img.height * 0.01),
            int(img.width * 0.93),
            int(img.height * 0.80),
        ),
        fill=(228, 180, 113, 30),
    )
    glow = glow.filter(ImageFilter.GaussianBlur(60 * SCALE // 4))
    img.alpha_composite(glow)


def main() -> None:
    ASSETS.mkdir(parents=True, exist_ok=True)
    img = rounded_gradient(SIZE)
    add_glow(img)
    draw = ImageDraw.Draw(img, "RGBA")

    cx = cy = SIZE / 2
    radius = SIZE * 0.335
    gold = (231, 184, 114, 255)
    gold_soft = (231, 184, 114, 95)
    ivory = (247, 243, 234, 255)
    coral = (255, 118, 101, 255)

    # Outer dial ring and an inner hairline.
    ring_w = max(1, int(SIZE * 0.014))
    draw.ellipse(
        (cx - radius, cy - radius, cx + radius, cy + radius),
        outline=gold,
        width=ring_w,
    )
    r2 = radius * 0.885
    draw.ellipse(
        (cx - r2, cy - r2, cx + r2, cy + r2),
        outline=gold_soft,
        width=max(1, int(SIZE * 0.004)),
    )

    # Twelve hour marks; cardinals are longer and brighter.
    for hour in range(12):
        angle = math.radians(hour * 30 - 90)
        major = hour % 3 == 0
        outer = radius * (0.80 if major else 0.835)
        inner = radius * (0.70 if major else 0.775)
        x1, y1 = cx + math.cos(angle) * inner, cy + math.sin(angle) * inner
        x2, y2 = cx + math.cos(angle) * outer, cy + math.sin(angle) * outer
        draw.line(
            (x1, y1, x2, y2),
            fill=gold if major else (231, 184, 114, 125),
            width=max(2, int(SIZE * (0.012 if major else 0.0055))),
        )

    def hand(angle_deg: float, length: float, width: int, color: tuple[int, int, int, int]) -> None:
        angle = math.radians(angle_deg - 90)
        x = cx + math.cos(angle) * length
        y = cy + math.sin(angle) * length
        draw.line((cx, cy, x, y), fill=color, width=width)
        rr = width / 2
        draw.ellipse((x - rr, y - rr, x + rr, y + rr), fill=color)
        draw.ellipse((cx - rr, cy - rr, cx + rr, cy + rr), fill=color)

    # Classic 10:10 composition.
    hand(300, radius * 0.50, int(SIZE * 0.030), ivory)
    hand(60, radius * 0.68, int(SIZE * 0.022), ivory)
    hand(184, radius * 0.76, max(2, int(SIZE * 0.009)), coral)

    # Second-hand tail.
    tail_angle = math.radians(184 + 180 - 90)
    draw.line(
        (
            cx,
            cy,
            cx + math.cos(tail_angle) * radius * 0.18,
            cy + math.sin(tail_angle) * radius * 0.18,
        ),
        fill=coral,
        width=max(2, int(SIZE * 0.009)),
    )

    # Center pin.
    pin = SIZE * 0.026
    draw.ellipse((cx - pin, cy - pin, cx + pin, cy + pin), fill=(12, 19, 32, 255))
    pin2 = SIZE * 0.012
    draw.ellipse((cx - pin2, cy - pin2, cx + pin2, cy + pin2), fill=gold)

    # Subtle rim highlight.
    draw.rounded_rectangle(
        (1, 1, SIZE - 2, SIZE - 2),
        radius=int(SIZE * 0.225),
        outline=(255, 255, 255, 18),
        width=max(1, int(SIZE * 0.004)),
    )

    img = img.resize((256, 256), Image.Resampling.LANCZOS)
    img.save(ASSETS / "icon-256.png")
    img.save(
        ASSETS / "icon.ico",
        format="ICO",
        sizes=[(16, 16), (24, 24), (32, 32), (48, 48), (64, 64), (128, 128), (256, 256)],
    )
    print(f"Wrote {ASSETS / 'icon.ico'}")


if __name__ == "__main__":
    main()
