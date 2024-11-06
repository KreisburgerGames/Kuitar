bpm = 184
pixelsPerBeat = 32
secondsPerBeat = 60 / bpm
offset = 0

while True:
	time = float(input("time: "))

	beat = time / secondsPerBeat

	print(round(beat * pixelsPerBeat) - offset)

	i = input("Press Enter to enter again. Enter Q to quit. ")
	if (i == "Q" or i == "q"):
		exit()
  

# gl mapping, ty for giving my game a try :)
