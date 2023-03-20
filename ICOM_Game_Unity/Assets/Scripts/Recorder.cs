﻿using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add this component to a GameObject to Record Mic Input 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Recorder : MonoBehaviour
{
    /// Audio Source to store Microphone Input, An AudioSource Component is required by default
    static AudioSource audioSource;
    /// The samples are floats ranging from -1.0f to 1.0f, representing the data in the audio clip
    static float[] samplesData;
    /// WAV file header size
    const int HEADER_SIZE = 44;


    /// What should the saved file name be, the file will be saved in Streaming Assets Directory
    [Tooltip("What should the saved file name be, the file will be saved in Streaming Assets Directory, Don't add .wav at the end")]
    public string fileName;

    public AudioPlayer audioPlayer;
    public bool recording;

    private void Update()
    {
        if (recording)
        {
            audioPlayer.recordedClipLength += Time.deltaTime;
        }
    }

    public void StartRecording()
    {
        audioPlayer.audioLoaded = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(Microphone.devices[0], false, 300, 44100);
        audioPlayer.recordedClipLength = 0;
        recording = true;
    }

    public void Save()
    {
        recording = false;

        while (!(Microphone.GetPosition(null) > 0)) { }
        samplesData = new float[audioSource.clip.samples * audioSource.clip.channels];
        audioSource.clip.GetData(samplesData, 0);
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".wav");
        // Delete the file if it exists.
        if (File.Exists(filePath))
        {
            Debug.Log("Deleting File");
            File.Delete(filePath);
        }
        try
        {
            WriteWAVFile(audioSource.clip, filePath);
            Debug.Log("File Saved Successfully at StreamingAssets/" + fileName + ".wav");
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogError("Please, Create a StreamingAssets Directory in the Assets Folder");
        } 
    }

    // WAV file format from http://soundfile.sapp.org/doc/WaveFormat/
    static void WriteWAVFile(AudioClip clip, string filePath)
    {
        float[] clipData = new float[clip.samples];

        //Create the file.
        using (Stream fs = File.Create(filePath))
        {
            int frequency = clip.frequency;
            int numOfChannels = clip.channels;
            int samples = clip.samples;
            fs.Seek(0, SeekOrigin.Begin);

            //Header

            // Chunk ID
            byte[] riff = Encoding.ASCII.GetBytes("RIFF");
            fs.Write(riff, 0, 4);

            // ChunkSize
            byte[] chunkSize = BitConverter.GetBytes((HEADER_SIZE + clipData.Length) - 8);
            fs.Write(chunkSize, 0, 4);

            // Format
            byte[] wave = Encoding.ASCII.GetBytes("WAVE");
            fs.Write(wave, 0, 4);

            // Subchunk1ID
            byte[] fmt = Encoding.ASCII.GetBytes("fmt ");
            fs.Write(fmt, 0, 4);

            // Subchunk1Size
            byte[] subChunk1 = BitConverter.GetBytes(16);
            fs.Write(subChunk1, 0, 4);

            // AudioFormat
            byte[] audioFormat = BitConverter.GetBytes(1);
            fs.Write(audioFormat, 0, 2);

            // NumChannels
            byte[] numChannels = BitConverter.GetBytes(numOfChannels);
            fs.Write(numChannels, 0, 2);

            // SampleRate
            byte[] sampleRate = BitConverter.GetBytes(frequency);
            fs.Write(sampleRate, 0, 4);

            // ByteRate
            byte[] byteRate = BitConverter.GetBytes(frequency * numOfChannels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
            fs.Write(byteRate, 0, 4);

            // BlockAlign
            ushort blockAlign = (ushort)(numOfChannels * 2);
            fs.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            // BitsPerSample
            ushort bps = 16;
            byte[] bitsPerSample = BitConverter.GetBytes(bps);
            fs.Write(bitsPerSample, 0, 2);

            // Subchunk2ID
            byte[] datastring = Encoding.ASCII.GetBytes("data");
            fs.Write(datastring, 0, 4);

            // Subchunk2Size
            byte[] subChunk2 = BitConverter.GetBytes(samples * numOfChannels * 2);
            fs.Write(subChunk2, 0, 4);

            // Data

            clip.GetData(clipData, 0);
            short[] intData = new short[clipData.Length];
            byte[] bytesData = new byte[clipData.Length * 2];

            int convertionFactor = 32767;

            for (int i = 0; i < clipData.Length; i++)
            {
                intData[i] = (short)(clipData[i] * convertionFactor);
                byte[] byteArr = new byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }
                
            fs.Write(bytesData, 0, bytesData.Length);
        }
    }
}