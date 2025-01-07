# Digit Recognizer with Multi-Class Classification

## How to Run

Download the game below:

For MacOS Silicon or MacOS Intel 64-bit: https://storage.googleapis.com/mnist-game/mnist-MacOS-silicon-intel64bit.app.zip

---

## About the Project

This project combines Unity and machine learning to create a digit recognition system that achieves **98% accuracy** using a pre-trained PyTorch model. The model was trained on the MNIST dataset, leveraging **fully connected dense layers** with **softmax output activation** for multi-class classification. 

In Unity, I implemented a **canvas** that allows users to draw digits, which are then classified in real-time by my **Supervised Learning Neural Network Library** (written in C#) utilizing the trained PyTorch model's weights and biases. To handle the noisier input from the Unity drawing canvas, I applied **data augmentation** during training to improve the model's robustness.

Additionally, I tested my custom **C# Supervised Learning Neural Network Library** for training but achieved a slightly lower accuracy of **93.1%** on the test set. As a result, I chose to use PyTorch for training while my library handled the real-time forward passes, loading of weights and biases, and overall neural network structure.

---

## Features

- **PyTorch Training**:
  - Model trained on MNIST dataset with data augmentation to adapt to noisy input.
  - Dense layers with softmax output activation for multi-class classification.
  - Achieved 98% accuracy on the test set.

- **Custom C# Neural Network Library**:
  - Built from scratch to handle real-time predictions and parameter loading.
  - Implements key neural network components:
    - **Forward passes**
    - **Backpropagation**
    - **Activation functions** (e.g., ReLU, Sigmoid, Softmax)
    - **Optimization techniques**
    - **Normalization** and **data preprocessing** (e.g., one-hot encoding, missing feature handling).
  - Handles importing weights and biases from PyTorch models through **CSV files**.

- **Unity Integration**:
  - Interactive **drawing canvas** for users to input digits.
  - Real-time digit recognition powered by pre-trained model weights and my neural network library.

---

## Challenges and Solutions

### Problem: Handwriting Differences and Drawing Panel Noise
- **Challenge**: The formatting of Unity's drawing panel differs from the MNIST dataset, leading to reduced accuracy.
- **Solution**:
  - Applied **data augmentation** using PyTorch's `transforms` library to introduce variations such as rotations, scaling, and added noise to mimic real-world inputs.
  - Planned future improvement: Generate additional training data directly from Unity's drawing canvas to better align the dataset with real-world usage.

### Problem: Loading Pre-Trained Weights into Unity
- **Challenge**: Incorporating PyTorch-trained weights and biases into Unity's environment while ensuring compatibility with my neural network library.
- **Solution**:
  - Extended my **C# Supervised Learning Neural Network Library** to load and process CSV files containing the model's parameters.
  - Developed robust file-handling logic to manage **cross-platform compatibility** of CSV formats between Python and Unity.

### Problem: Combining PyTorch and Custom Neural Network Code
- **Challenge**: Using PyTorch for training while ensuring the custom library functions effectively for predictions in Unity.
- **Solution**:
  - Treated my C# library as a "neural network shell," handling forward passes, neurons, layers, and predictions.
  - Utilized PyTorch for the heavy lifting during training but seamlessly integrated its parameters into the Unity project for real-time inference.

---

## Future Improvements

- **Custom Data Collection**:
  - Create a dataset of handwritten digits directly from the Unity drawing canvas to improve real-world accuracy.
  - Include additional data augmentation specific to Unity's panel.

- **Library Enhancements**:
  - Optimize the C# library for faster loading of weights and biases.
  - Add support for additional activation functions and optimizers.

- **Extended Use Cases**:
  - Expand the application to recognize more complex datasets (e.g., multi-character input or custom alphabets).

---

## Repository Overview

### PyTorch Training Code
- Located in Jupyter notebooks.
- Handles:
  - Dataset loading (MNIST)
  - Model architecture and training
  - Data augmentation with the `transforms` library
  - Exporting weights and biases to CSV for Unity.

### C# Supervised Learning Neural Network Library
- Custom-built library available here: [MLLibrary on GitHub](https://github.com/yckamra/MLLibrary).
- Handles:
  - Loading weights and biases from CSV.
  - Forward passes and inference for real-time Unity integration.

### Unity Project
- Interactive digit recognition app:
  - Draw digits on the canvas and see real-time predictions.
  - Integrates the C# library with pre-trained PyTorch parameters.

---

## Acknowledgments

Special thanks to the open-source community for the MNIST dataset and PyTorch. This project bridges cutting-edge machine learning with interactive Unity applications, demonstrating the versatility of combining multiple technologies.

---
