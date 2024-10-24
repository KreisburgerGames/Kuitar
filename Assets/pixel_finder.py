pixels_per_beat = 4
beats_per_bar = 4

bar = int(input("bar: "))
beat = int(input("beat: "))
tick = int(input("tick: "))
subtick = int(input("sub-tick: "))

pixel = ((bar - 1) * (pixels_per_beat * beats_per_bar)) + (beat * pixels_per_beat) + tick - pixels_per_beat - 1 + subtick - 1
print(pixel)

input("Press Enter to Exit.")