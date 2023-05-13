using System;
using System.Collections.Generic;
using System.Linq;

public class HuffmanTree
{
    private readonly HuffmanNode _rootNode;
    private readonly Dictionary<char, string> _encodingDictionary;

    public HuffmanTree(byte[] data)
    {
        // Calculate the frequency of each byte in the data
        Dictionary<byte, int> frequencyTable = new Dictionary<byte, int>();
        foreach (byte b in data)
        {
            if (!frequencyTable.ContainsKey(b))
            {
                frequencyTable[b] = 0;
            }
            frequencyTable[b]++;
        }

        // Build a Huffman tree from the frequency table
        List<HuffmanNode> nodes = frequencyTable
            .Select(pair => new HuffmanNode(pair.Key, pair.Value))
            .ToList();
        while (nodes.Count > 1)
        {
            nodes.Sort((a, b) => a.Frequency - b.Frequency);
            HuffmanNode newNode = new HuffmanNode(nodes[0], nodes[1]);
            nodes.RemoveRange(0, 2);
            nodes.Add(newNode);
        }

        _rootNode = nodes[0];

        // Build an encoding dictionary from the Huffman tree
        _encodingDictionary = new Dictionary<char, string>();
        BuildEncodingDictionary(_rootNode, "");
    }

    public Dictionary<char, string> EncodingDictionary => _encodingDictionary;

    public byte[] Compress(byte[] data)
    {
        // Encode the data using the Huffman tree
        List<bool> encodedBits = new List<bool>();
        foreach (byte b in data)
        {
            string encodedValue = _encodingDictionary[(char)b];
            foreach (char c in encodedValue)
            {
                encodedBits.Add(c == '1');
            }
        }

        // Convert the encoded bits to bytes
        byte[] compressedData = new byte[(encodedBits.Count + 7) / 8];
        for (int i = 0; i < encodedBits.Count; i++)
        {
            if (encodedBits[i])
            {
                compressedData[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }
        }

        return compressedData;
    }

    public byte[] Decompress(byte[] data)
    {
        // Convert the compressed data to encoded bits
        List<bool> encodedBits = new List<bool>();
        for (int i = 0; i < data.Length; i++)
        {
            byte b = data[i];
            for (int j = 0; j < 8; j++)
            {
                encodedBits.Add((b & (1 << (7 - j))) != 0);
            }
        }

        // Decode the encoded bits using the Huffman tree
        List<byte> decodedBytes = new List<byte>();
        HuffmanNode currentNode = _rootNode;
        foreach (bool bit in encodedBits)
        {
            if (bit)
            {
                currentNode = currentNode.RightChild;
            }
            else
            {
                currentNode = currentNode.LeftChild;
            }

            if (currentNode.Value.HasValue)
            {
                decodedBytes.Add(currentNode.Value.Value);
                currentNode = _rootNode;
            }
        }

        return decodedBytes.ToArray();
    }

    private void BuildEncodingDictionary(HuffmanNode node, string prefix)
    {
        if (node.Value.HasValue)
        {
            _encodingDictionary[node.Value.Value] = prefix;
        }
        else
        {
            BuildEncodingDictionary(node.LeftChild, prefix + "0");
            BuildEncodingDictionary(node.RightChild, prefix + "1");
        }
    }

    private class HuffmanNode
    {
        public HuffmanNode(byte value)
        {
            Value = value;
            Frequency = 1;
        }
        public HuffmanNode(HuffmanNode leftChild, HuffmanNode rightChild)
        {
            LeftChild = leftChild;
            RightChild = rightChild;
            Frequency = leftChild.Frequency + rightChild.Frequency;
        }

        public byte? Value { get; }

        public int Frequency { get; }

        public HuffmanNode LeftChild { get; }

        public HuffmanNode RightChild { get; }
    }
}