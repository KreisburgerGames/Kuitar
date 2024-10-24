bpm = 155
pixelsPerBeat = 4
secondsPerBeat = 60 / bpm

time = float(input("time: "))

beat = time / secondsPerBeat

print(round(beat * pixelsPerBeat))

input("Press Enter to Exit")