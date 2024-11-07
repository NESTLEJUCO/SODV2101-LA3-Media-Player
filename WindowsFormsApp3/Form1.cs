//Learning Activity 3: Media Player
//SODV2101 Rapid Application Development 24SEPMNFS1
//Submitted By: Nestle Juco
using AxWMPLib;
using NAudio.Wave;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class MediaPlayer : Form
    {
        Image img;
        WaveOutEvent waveOut;     
        AudioFileReader audioFile; 
        private string lastLoadedFile; 
        private enum MediaType { None, WAV, MP3MP4 } 
        private MediaType lastLoadedFileType = MediaType.None; 

        public MediaPlayer()
        {
            InitializeComponent();
        }

        // Play Button
        private void btnPlay_Click(object sender, EventArgs e)
        {
            // For WAV audio (NAudio)
            if (lastLoadedFileType == MediaType.WAV && waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }

                // Paused and resume playback
                if (waveOut.PlaybackState == PlaybackState.Paused)
                {
                    waveOut.Play();
                }
                else
                {
                    // Start from the beginning if Stop
                    audioFile.Position = 0;
                    waveOut.Play();
                }
            }
            // For MP3/MP4 media (Windows Media Player)
            else if (lastLoadedFileType == MediaType.MP3MP4 && !string.IsNullOrEmpty(lastLoadedFile))
            {
                if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
                {
                    return;  
                }

                // Paused and resume playback
                if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
                {
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
                else
                {
                    // Start from the beginning if Stop
                    axWindowsMediaPlayer1.URL = lastLoadedFile;
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                }
            }

            // For Double Audio
            else if (AudioOut1 != null && AudioOut2 != null)
            {
                // Check for AudioOut1 (for the first audio file)
                if (AudioOut1.PlaybackState == PlaybackState.Playing && AudioOut2.PlaybackState == PlaybackState.Playing)
                {
                    // Both are already playing
                    return;
                }

                // Check if AudioOut1 is paused
                if (AudioOut1.PlaybackState == PlaybackState.Paused)
                {
                    AudioOut1.Play();
                }
                else if (AudioOut1.PlaybackState == PlaybackState.Stopped)
                {
                    // Start from the beginning if stopped
                    Audio1.CurrentTime = TimeSpan.Zero;
                    AudioOut1.Play();
                }

                // Check for AudioOut2 (for the second audio file)
                if (AudioOut2.PlaybackState == PlaybackState.Paused)
                {
                    AudioOut2.Play();
                }
                else if (AudioOut2.PlaybackState == PlaybackState.Stopped)
                {
                    // Start from the beginning if stopped
                    Audio2.CurrentTime = TimeSpan.Zero;
                    AudioOut2.Play();
                }
            }
            else
            {
                MessageBox.Show("No media file loaded to play.");
            }
        }

        // Stop Button
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMedia();
        }

        // Pause Button
        private void btnPause_Click(object sender, EventArgs e)
        {
            // Check of WAV file is playing
            if (lastLoadedFileType == MediaType.WAV && waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    waveOut.Pause(); 
                }
            }
            // Check if MP3/MP4 file is playing
            else if (lastLoadedFileType == MediaType.MP3MP4 && axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
            }

            if (AudioOut1 != null || AudioOut2 != null)
            {
                AudioOut1.Pause();
                AudioOut2.Pause();
            }

        }

        // Stop Media Method
        private void StopMedia()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                audioFile.Position = 0;
            }

            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPlaying ||
                axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                axWindowsMediaPlayer1.Ctlcontrols.stop();
                axWindowsMediaPlayer1.URL = null;
            }

            if (AudioOut1 != null)
            {
                AudioOut1.Stop();
            }

            if (AudioOut2 != null)
            {
                AudioOut2.Stop();
            }
        }


        // Play WAV Audio Button
        private void btnAudioWAV_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV Audio | *.wav"; 

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StopMedia();
                audioFile = new AudioFileReader(openFileDialog.FileName);
                waveOut = new WaveOutEvent();
                waveOut.Init(audioFile);
                waveOut.Play();

                lastLoadedFile = openFileDialog.FileName;
                lastLoadedFileType = MediaType.WAV;
            }
        }


        // Play Media Button (MP3, MP4 or AVI)
        private void btnMedia_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Media Files|*.mp3;*.mp4;*.avi"; 

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                StopMedia();
                axWindowsMediaPlayer1.URL = openFileDialog.FileName;
                axWindowsMediaPlayer1.Ctlcontrols.play();

                lastLoadedFile = openFileDialog.FileName;
                lastLoadedFileType = MediaType.MP3MP4;

            }
        }


        //Play Double Audio Button
        private WaveStream Audio1;
        private WaveStream Audio2;
        private WaveOut AudioOut1;
        private WaveOut AudioOut2;
        private void btnDouble_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Audio Files|*.wav;*.mp3|All Files|*.*";
            openFileDialog1.Title = "Select the first audio file";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StopMedia();
                string audioFilePath1 = openFileDialog1.FileName;

                OpenFileDialog openFileDialog2 = new OpenFileDialog();
                openFileDialog2.Filter = "Audio Files|*.wav;*.mp3|All Files|*.*";
                openFileDialog2.Title = "Select the second audio file";

                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    string audioFilePath2 = openFileDialog2.FileName;

                    if (AudioOut1 != null)
                    {
                        AudioOut1.Stop();
                        AudioOut1.Dispose();
                        Audio1.Dispose();
                    }

                    if (AudioOut2 != null)
                    {
                        AudioOut2.Stop();
                        AudioOut2.Dispose();
                        Audio2.Dispose();
                    }

                    Audio1 = new AudioFileReader(audioFilePath1);
                    Audio2 = new AudioFileReader(audioFilePath2);
                    AudioOut1 = new WaveOut();
                    AudioOut2 = new WaveOut();

                    AudioOut1.Init(Audio1);
                    AudioOut2.Init(Audio2);

                    Audio1.CurrentTime = TimeSpan.Zero;
                    Audio2.CurrentTime = TimeSpan.Zero;

                    AudioOut1.Play();
                    AudioOut2.Play();

                    SaveAudioPaths(audioFilePath1, audioFilePath2);
                }
            }
        }

        // Save audio paths to a file
        private void SaveAudioPaths(string path1, string path2)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audio_paths.txt");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(path1);
                writer.WriteLine(path2);
            }
        }

        // Change Skin Button
        private Image originalImage;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Load the image and store it as the original image
                img = Image.FromFile(ofd.FileName);
                originalImage = (Image)img.Clone(); 
                pictureMain.Image = img;
            }
        }

        // Rotate Skin Image on Click
        private void pictureMain_Click(object sender, EventArgs e)
        {
            if (pictureMain.Image != null)
            {
                Image temp = pictureMain.Image;
                temp.RotateFlip(RotateFlipType.Rotate90FlipXY);
                pictureMain.Image = temp;
            }

            else
            {
                MessageBox.Show("Default skin cannot be rotated. Please load new skin image first.");
            }
        }

        // Skin Texture Apply Button
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (pictureMain.Image != null)
            {
                if (comboBox1.Text == "No Filter")
                {
                    pictureMain.Image = (Image)originalImage.Clone(); 
                    return; 
                }

                // Create a Bitmap to hold the current image
                Bitmap modifiedBmp = new Bitmap(pictureMain.Image);

                Rectangle rect = new Rectangle(0, 0, modifiedBmp.Width, modifiedBmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    modifiedBmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, modifiedBmp.PixelFormat);

                IntPtr ptr = bmpData.Scan0;

                int bytes = Math.Abs(bmpData.Stride) * modifiedBmp.Height;
                byte[] rgbValues = new byte[bytes];

                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                // Apply texture based on the selected option
                if (comboBox1.Text == "Grayscale")
                {
                    // Grayscale conversion
                    for (int i = 0; i < rgbValues.Length; i += 4)
                    {
                        byte blue = rgbValues[i];
                        byte green = rgbValues[i + 1];
                        byte red = rgbValues[i + 2];

                        byte gray = (byte)(0.299 * red + 0.587 * green + 0.114 * blue);

                        rgbValues[i] = gray;      // Blue
                        rgbValues[i + 1] = gray;  // Green
                        rgbValues[i + 2] = gray;  // Red
                    }
                }
                else if (comboBox1.Text == "Sepia")
                {
                    // Sepia conversion
                    for (int i = 0; i < rgbValues.Length; i += 4)
                    {
                        byte blue = rgbValues[i];
                        byte green = rgbValues[i + 1];
                        byte red = rgbValues[i + 2];

                        byte tr = (byte)Math.Min(0.393 * red + 0.769 * green + 0.189 * blue, 255);
                        byte tg = (byte)Math.Min(0.349 * red + 0.686 * green + 0.168 * blue, 255);
                        byte tb = (byte)Math.Min(0.272 * red + 0.534 * green + 0.131 * blue, 255);

                        rgbValues[i] = tb;      // Blue
                        rgbValues[i + 1] = tg;  // Green
                        rgbValues[i + 2] = tr;  // Red
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
                modifiedBmp.UnlockBits(bmpData);
                pictureMain.Image = modifiedBmp;
            }
            else
            {
                MessageBox.Show("Default skin cannot be filtered. Please load new skin image first.");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dispose of the NAudio resources properly when closing the form
            if (waveOut != null)
            {
                waveOut.Dispose();
            }
            if (audioFile != null)
            {
                audioFile.Dispose();
            }
            if (AudioOut1 != null)
            {
                AudioOut1.Stop();
                AudioOut1.Dispose();
                Audio1.Dispose();
            }
            if (AudioOut2 != null)
            {
                AudioOut2.Stop();
                AudioOut2.Dispose();
                Audio2.Dispose();
            }
        }
    }
}
