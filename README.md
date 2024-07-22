# AdvancedEncryptor

**AdvancedEncryptor** is a multi-page WPF application that provides encryption and decryption capabilities for both text and files. It features custom implementations of several cryptographic algorithms contained within the application code.

## Supported Algorithms

- **AES (Advanced Encryption Standard)**
  - Key Lengths: 128, 192, and 256 bits
  - Modes of Operation: ECB and CBC
  - Supports parallel encryption of blocks to accelerate the encryption process.

- **S-DES (Simplified Data Encryption Standard)**
  - 10-bit key, 64-bit block size
  - Supports parallel encryption of blocks for improved efficiency.

- **LFSR (Linear Feedback Shift Register)**
  - As a stream cipher, in addition to text and files, it also supports encryption of a plain sequence of bits

- **GOST 28147-89**
  - Modes of Operation: ECB, XOR, and CFB.


## Usage

To use AdvancedEncryptor, follow these steps:

1. **Install Visual Studio**:
   - Ensure you have Visual Studio installed on your machine. You can download it from [Visual Studio](https://visualstudio.microsoft.com/).

2. **Install .NET Desktop Development Workload**:
   - During the Visual Studio installation, select the `.NET Desktop Development` workload. If Visual Studio is already installed, you can add this workload via the Visual Studio Installer.

3. **Clone the Repository**:
   - Open a terminal and run the following command to clone the repository:
     ```sh
     git clone https://github.com/CatAndDog53/AdvancedEncryptor.git
     ```

4. **Open the Solution**:
   - Open Visual Studio.
   - Select `File > Open > Project/Solution`.
   - Navigate to the cloned repository directory and open the `AdvancedEncryptor.sln` file.

5. **Build the Solution**:
   - In Visual Studio, build the solution by selecting `Build > Build Solution` or pressing `Ctrl+Shift+B`.

6. **Run the Application**:
   - Start the application by selecting `Debug > Start Debugging` or pressing `F5`.
   - The AdvancedEncryptor WPF application will launch, allowing you to select algorithms, modes, and key lengths to encrypt and decrypt text and files.
