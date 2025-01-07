using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ML;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class SceneObserverScript : MonoBehaviour
{
    public static SceneObserverScript Instance; // Singleton pattern
    public DrawingManager drawingManager;
    private SupervisedNetwork network = null;
    public double[,] featureData;
    public double[,] targetData;

    public TextMeshProUGUI textMeshPro;

    public TextMeshProUGUI debugger;

    public double[,] inputData; // TODO: for testing
    public int index = 0; // TODO: for testing

    public string currentGuess = "";

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        textMeshPro.text = "Left click to draw and 'c' to clear";

        // (1) create the model

        this.network = CreateNetwork();

        // (2) get the data from save file

        FillWeightsAndBiases(ref network);

        string featureDataPath = "Assets/feature_test.csv"; // TODO: for testing
        featureData = LoadCsvToDoubleArray(featureDataPath); // TODO: for testing
        string targetDataPath = "Assets/target_test.csv"; // TODO: for testing
        double[,] targetData = LoadCsvToDoubleArray(targetDataPath); // TODO: for testing

        // double accuracy = ModelAccuracy(network, featureData, targetData); // TODO: for testing
    }


    public double ModelAccuracy(SupervisedNetwork network, double[,] input, double[,] output)
    {

        int rows = input.GetLength(0);
        int totalRight = 0;

        for (int i = 0; i < rows; i++)
        {
            double[,] yPred = network.Predict(NetworkFunctions.GetARow(input, i));

            NetworkFunctions.SoftMaxMatrixToOneHot(yPred);

            double[,] transposeYPred = NetworkFunctions.Transpose(yPred);

            string guess = OneHotDecoderMNIST(transposeYPred);
            string actual = OneHotDecoderMNISTMakeNormal(NetworkFunctions.GetARow(output, i));
            if (guess == actual)
            {
                totalRight++;
            }
        }
        double error = totalRight / (double)rows;

        return error;
    }

    private static string OneHotDecoderMNISTMakeNormal(double[,] input)
    {
        int index = 0;
        for (int i = 0; i < input.GetLength(1); i++)
        {
            if (input[0, i] == 1)
            {
                index = i;
            }
        }
        string guess = "";
        switch (index)
        {
            case 0:
                guess = "7";
                break;
            case 1:
                guess = "2";
                break;
            case 2:
                guess = "1";
                break;
            case 3:
                guess = "0";
                break;
            case 4:
                guess = "4";
                break;
            case 5:
                guess = "9";
                break;
            case 6:
                guess = "5";
                break;
            case 7:
                guess = "6";
                break;
            case 8:
                guess = "3";
                break;
            case 9:
                guess = "8";
                break;
        }
        return guess;
    }

    private static string OneHotDecoderMNIST(double[,] input)
    {
        int index = 0;
        for (int i = 0; i < input.GetLength(0); i++) // TODO: flip these to predict
        {
            if (input[i, 0] == 1) // TODO: flip these to predict
            {
                index = i;
            }
        }
        string guess = "";
        switch (index)
        {
            case 0:
                guess = "0";
                break;
            case 1:
                guess = "1";
                break;
            case 2:
                guess = "2";
                break;
            case 3:
                guess = "3";
                break;
            case 4:
                guess = "4";
                break;
            case 5:
                guess = "5";
                break;
            case 6:
                guess = "6";
                break;
            case 7:
                guess = "7";
                break;
            case 8:
                guess = "8";
                break;
            case 9:
                guess = "9";
                break;
        }
        return guess;
    }

    
         void FillWeightsAndBiases(ref SupervisedNetwork network)
        {

            List<List<string>> weightsForFirstDenseList = new List<List<string>>();
            List<List<string>> biasesForFirstDenseList = new List<List<string>>();
            List<List<string>> weightsForSecondDenseList = new List<List<string>>();
            List<List<string>> biasesForSecondDenseList = new List<List<string>>();
            List<List<string>> weightsForThirdDenseList = new List<List<string>>();
            List<List<string>> biasesForThirdDenseList = new List<List<string>>();

            double[,] weightsForFirstDense = null;
            double[,] biasesForFirstDense = null;
            double[,] weightsForSecondDense = null;
            double[,] biasesForSecondDense = null;
            double[,] weightsForThirdDense = null;
            double[,] biasesForThirdDense = null;

        /* // NO LONGER USING THIS
        string weight1CSV = "Assets/StreamingAssets/l1_weights.csv";
        string bias1CSV = "Assets/StreamingAssets/l1_biases.csv";
        string weight2CSV = "Assets/StreamingAssets/l2_weights.csv";
        string bias2CSV = "Assets/StreamingAssets/l2_biases.csv";
        string weight3CSV = "Assets/StreamingAssets/l3_weights.csv";
        string bias3CSV = "Assets/StreamingAssets/l3_biases.csv";
        */
    
    string weight1CSV = Path.Combine(Application.streamingAssetsPath, "l1_weights.csv");
    string bias1CSV = Path.Combine(Application.streamingAssetsPath, "l1_biases.csv");
    string weight2CSV = Path.Combine(Application.streamingAssetsPath, "l2_weights.csv");
    string bias2CSV = Path.Combine(Application.streamingAssetsPath, "l2_biases.csv");
    string weight3CSV = Path.Combine(Application.streamingAssetsPath, "l3_weights.csv");
    string bias3CSV = Path.Combine(Application.streamingAssetsPath, "l3_biases.csv");

    // (3) put the data into the model

    // fill the lists with the csv files data
    DataFunctions.LoadCSV(weightsForFirstDenseList, weight1CSV);
    DataFunctions.LoadCSV(biasesForFirstDenseList, bias1CSV);
    DataFunctions.LoadCSV(weightsForSecondDenseList, weight2CSV);
    DataFunctions.LoadCSV(biasesForSecondDenseList, bias2CSV);
    DataFunctions.LoadCSV(weightsForThirdDenseList, weight3CSV);
    DataFunctions.LoadCSV(biasesForThirdDenseList, bias3CSV);

    // convert the lists to double[,]
    weightsForFirstDense = DataFunctions.StringMatrixToDoubleMatrix(weightsForFirstDenseList);
    biasesForFirstDense = DataFunctions.StringMatrixToDoubleMatrix(biasesForFirstDenseList);
    weightsForSecondDense = DataFunctions.StringMatrixToDoubleMatrix(weightsForSecondDenseList);
    biasesForSecondDense = DataFunctions.StringMatrixToDoubleMatrix(biasesForSecondDenseList);
    weightsForThirdDense = DataFunctions.StringMatrixToDoubleMatrix(weightsForThirdDenseList);
    biasesForThirdDense = DataFunctions.StringMatrixToDoubleMatrix(biasesForThirdDenseList);

    UpdateNetworkWeightsAndBiasesFromLoadedData(ref network, ref weightsForFirstDense,
        ref biasesForFirstDense, ref weightsForSecondDense, ref biasesForSecondDense, ref weightsForThirdDense, ref biasesForThirdDense);

}


    void UpdateNetworkWeightsAndBiasesFromLoadedData(ref SupervisedNetwork network, ref double[,] weightsForFirstDense, ref double[,] biasesForFirstDense,
        ref double[,] weightsForSecondDense, ref double[,] biasesForSecondDense, ref double[,] weightsForThirdDense, ref double[,] biasesForThirdDense)
    {
        if (network.layers[0] is Dense layerDense)
        {
            layerDense.weights = weightsForFirstDense;
            layerDense.biases = biasesForFirstDense;
        }
        if (network.layers[2] is Dense layerDense2)
        {
            layerDense2.weights = weightsForSecondDense;
            layerDense2.biases = biasesForSecondDense;
        }
        if(network.layers[4] is Dense layerDense3)
        {
            layerDense3.weights = weightsForThirdDense;
            layerDense3.biases = biasesForThirdDense;
        }
    }

    SupervisedNetwork CreateNetwork()
    {
        SupervisedNetwork network = new SupervisedNetwork();
        network.layers = new List<Layer> {
                new Dense(784, 128, "HeInitialization"),
                new ReLULayer(),
                new Dense(64, 64, "HeInitialization"),
                new ReLULayer(),
                new Dense(64, 10, "XavierInitialization"),
                new SoftMaxLayer(),
                new SoftMaxCrossEntropyLayer()
        };
        return network;
    }

    // Update is called once per frame
    void Update()
    {

        //Dense myDense = network.layers[0] as Dense;

        //debugger.text = myDense.weights[0,0].ToString();

        string[] sayings = new string[] { "Is it a ", "It is probably a ", "I'm thinking it's a "};
        System.Random random = new System.Random();
        //this.inputData = NetworkFunctions.GetARow(featureData, index); // TODO: for testing
        // ----------------------------------------
        // Use the model to predict the digit drawn
        // ----------------------------------------
        if (Input.GetMouseButton(0))
        {
            // (1) get the input from the Drawing Manager and put it onto the game view
            double[,] input = drawingManager.GetGrayscaleArray();
            if (input != null)
            {
                string guess = "";
                double[,] prediction = network.Predict(input);
                //Debug.Log("[");
                for (int j = 0; j < prediction.GetLength(0); j++)
                {
                    //Debug.Log(prediction[j, 0]);
                }
                //Debug.Log("]");
                NetworkFunctions.SoftMaxMatrixToOneHot(prediction);

                guess = OneHotDecoderMNIST(prediction);
                if (currentGuess != guess)
                {
                    int rand = random.Next(0, 3);
                    currentGuess = guess;
                    textMeshPro.text = sayings[rand] + guess + "?";
                }
                else
                {


                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            currentGuess = "";
            textMeshPro.text = "Left click to draw and 'c' to clear";
        }

    }

    public static double[,] LoadCsvToDoubleArray(string filePath)
    {
        // Read all lines from the CSV file
        string[] lines = File.ReadAllLines(filePath);

        // Determine the number of rows and columns
        int rows = lines.Length;
        int cols = lines[0].Split(',').Length;

        // Create a double[,] array with the appropriate size
        double[,] result = new double[rows, cols];

        // Parse the CSV data into the double[,] array
        for (int i = 0; i < rows; i++)
        {
            string[] values = lines[i].Split(',');

            for (int j = 0; j < cols; j++)
            {
                // Convert the string to a double and assign it to the array
                result[i, j] = double.Parse(values[j]);
            }
        }

        return result;
    }
}
