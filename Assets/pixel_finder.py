pixels_per_beat = 4
beats_per_bar = 4

m = int(input("bar: "))
b = int(input("beat: "))
t = int(input("tick: "))

pixel = ((m - 1) * (pixels_per_beat * beats_per_bar)) + (b * pixels_per_beat) + t - 1 - pixels_per_beat
print(pixel)