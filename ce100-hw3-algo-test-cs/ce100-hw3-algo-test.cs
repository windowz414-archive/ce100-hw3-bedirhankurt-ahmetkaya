/**
 *
 * Copyright (C) 2023 Beru Hinode on behalf of Ahmet KAYA
 *
 * SPDX-License-Identifier: GPL-2.0-only
 */

using ce100_hw3_algo_lib;
using System.IO;
using System.Text;
using Xunit;

public class HuffmanTreeTests
{
    [Fact]
    public void CompressAndDecompressTest()
    {
        // Arrange
        string musicFilePath = @"../../../grind.mp3";
        string randomFilePath = @"random.txt";

        // Generate a random text file filled with Lorem Ipsum text until the music and text files are of the same size
        int musicFileSize = (int)new FileInfo(musicFilePath).Length;
        string randomText = LoremIpsumGenerator.GenerateLoremIpsum(musicFileSize);
        File.WriteAllText(randomFilePath, randomText);

        // Compress the music and random files using Huffman coding
        string compressedMusicFilePath = @"compressedgrind.mp3";
        string compressedRandomFilePath = @"compressedrandom.txt";
        byte[] musicinput = File.ReadAllBytes(musicFilePath);
        byte[] randominput = File.ReadAllBytes(randomFilePath);
        byte[] musicoutput = HuffmanTree.Compress(musicinput);
        HuffmanTree.Compress(randomFilePath, compressedRandomFilePath);

        // Decompress the compressed music and random files
        string decompressedMusicFilePath = @"decompressedgrind.mp3";
        string decompressedRandomFilePath = @"decompressedrandom.txt";
        HuffmanTree.Decompress(compressedMusicFilePath, decompressedMusicFilePath);
        HuffmanTree.Decompress(compressedRandomFilePath, decompressedRandomFilePath);

        // Assert that the decompressed files are the same as the original files
        Assert.True(AreFilesEqual(musicFilePath, decompressedMusicFilePath));
        Assert.True(AreFilesEqual(randomFilePath, decompressedRandomFilePath));
    }

    public static class LoremIpsumGenerator
    {
        private static readonly Random _random = new Random();

        private static readonly string[] _words = new string[] {
        "Lorem", "ipsum", "dolor", "sit", "amet", "consectetur",
        "adipiscing", "elit", "sed", "do", "eiusmod", "tempor",
        "incididunt", "ut", "labore", "et", "dolore", "magna",
        "aliqua", "Ut", "enim", "ad", "minim", "veniam",
        "quis", "nostrud", "exercitation", "ullamco", "laboris",
        "nisi", "ut", "aliquip", "ex", "ea", "commodo",
        "consequat", "Duis", "aute", "irure", "dolor", "in",
        "reprehenderit", "in", "voluptate", "velit", "esse",
        "cillum", "dolore", "eu", "fugiat", "nulla", "pariatur",
        "Excepteur", "sint", "occaecat", "cupidatat", "non",
        "proident", "sunt", "in", "culpa", "qui", "officia",
        "deserunt", "mollit", "anim", "id", "est", "laborum"
    };

        public static string GenerateLoremIpsum(int length)
        {
            StringBuilder sb = new StringBuilder(length);

            while (sb.Length < length)
            {
                sb.Append(_words[_random.Next(_words.Length)]);
                sb.Append(" ");
            }

            return sb.ToString().Substring(0, length);
        }
    }


    private bool AreFilesEqual(string filePath1, string filePath2)
    {
        byte[] file1Bytes = File.ReadAllBytes(filePath1);
        byte[] file2Bytes = File.ReadAllBytes(filePath2);

        if (file1Bytes.Length != file2Bytes.Length)
        {
            return false;
        }

        for (int i = 0; i < file1Bytes.Length; i++)
        {
            if (file1Bytes[i] != file2Bytes[i])
            {
                return false;
            }
        }

        return true;
    }
}
