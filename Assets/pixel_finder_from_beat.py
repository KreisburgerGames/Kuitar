pixels_per_beat = 4
beats_per_bar = 4
offset = 0

while True:
    bar = int(input("bar: "))
    beat = int(input("beat: "))
    tick = int(input("tick: "))
    subtick = int(input("sub-tick: "))

    pixel = ((bar - 1) * (pixels_per_beat * beats_per_bar)) + (beat * pixels_per_beat) + tick - pixels_per_beat - 1 + subtick - 1 - offset
    print(pixel)

    i = input("Press Enter to enter again. Enter Q to quit. ")
    if (i == "Q" or i == "q"):
        exit()
        
        
        





# gl mapping, ty for giving my game a try :)
# PS: I highly recomend using pixel_finder_timecode.py instead, thank me later