import wave
import numpy as np
from PIL import Image
import os

def visualize_audio_peaks(file_path, pixels_per_beat, bpm, save_file_path):
    """
    Create one or more 4000x4 image files with pixels on audio peaks.

    :param file_path: Path to the WAV audio file.
    :param pixels_per_beat: Number of pixels to represent each beat.
    :param bpm: Beats per minute of the audio.
    """
    # Open the WAV file
    with wave.open(file_path, 'rb') as wav_file:
        # Extract parameters
        n_channels = wav_file.getnchannels()
        sample_rate = wav_file.getframerate()
        n_samples = wav_file.getnframes()
        
        # Read the audio data
        signal_wave = wav_file.readframes(n_samples)
        signal_array = np.frombuffer(signal_wave, dtype=np.int16)

        # If stereo, split channels
        if n_channels == 2:
            left_channel = signal_array[0::2]
            right_channel = signal_array[1::2]
            signal_array = left_channel  # Use only one channel for simplicity

        # Normalize the signal
        signal_array = signal_array / np.max(np.abs(signal_array))

        # Find peaks
        threshold = 0.05  # Adjust this value as necessary
        peaks = np.where((signal_array[1:-1] > signal_array[:-2]) & 
                         (signal_array[1:-1] > signal_array[2:]) & 
                         (signal_array[1:-1] > threshold))[0] + 1

        # Print the number of peaks detected
        print(f"Number of peaks detected: {len(peaks)}")
        print(f"Peak indices: {peaks[:10]}")  # Print the first 10 peaks for inspection

        # Calculate the number of beats and the corresponding pixel positions
        total_duration = n_samples / sample_rate  # Total duration in seconds
        total_beats = (bpm / 60) * total_duration  # Total number of beats
        pixels_per_beat = int(pixels_per_beat)  # Ensure it's an integer

        # Initialize image parameters
        img_width = 4000
        img_height = 4
        image_index = 1
        current_image = np.zeros((img_height, img_width, 4), dtype=np.uint8)  # RGBA image

        # Set pixels based on peaks
        for peak in peaks:
            # Calculate the corresponding pixel position
            time_in_seconds = peak / sample_rate  # Time of the peak in seconds
            beat_index = int((time_in_seconds * (bpm / 60)))  # Convert time to beat index
            pixel_position = beat_index * pixels_per_beat
            
            if pixel_position >= img_width:
                continue  # Skip if the pixel position exceeds the image width

            # Set the pixel color (e.g., red for peaks) and set alpha to 255 (opaque)
            current_image[:, pixel_position] = [255, 0, 0, 255]  # Red color for peaks with full opacity

        # Save the image only if there are peaks
        if np.any(current_image):
            img = Image.fromarray(current_image)
            img.save(save_file_path + f'audio_peaks_image_{image_index}.png')
            print("Image saved at " + save_file_path + f"audio_peaks_image_{image_index}.png")

# Example usage
if __name__ == "__main__":
    save_file_path = 'C:/Users/Dylan/Kuitar/Assets/StreamingAssets/Songs/HUSH/easy/'
    audio_file_path = 'C:/Users/Dylan/Kuitar/Assets/StreamingAssets/Songs/HUSH/HUSH.wav'  # Replace with your audio file path
    pixels_per_beat = 32  # Number of pixels per beat
    bpm = 184  # Beats per minute
    visualize_audio_peaks(audio_file_path, pixels_per_beat, bpm, save_file_path)