using Microsoft.Research.SEAL;

//https://github.com/microsoft/SEAL/blob/main/dotnet/examples/1_BFV_Basics.cs
//https://www.researchgate.net/publication/322131395_Using_Homomorphic_Cryptographic_Solutions_on_E-voting_Systems
namespace Voting.Shared
{
    public class SealManager
    {
        public PublicKey PublicKey { get; set; }

        public SEALContext Context { get; private set; }

        private readonly SecretKey secretKey;
        public SealManager()
        {

            Context = new SEALContext(GetParams());
            using KeyGenerator keygen = new KeyGenerator(Context);
            secretKey = keygen.SecretKey;
            keygen.CreatePublicKey(out PublicKey publicKey);
            this.PublicKey = publicKey;
        }

        private EncryptionParameters GetParams()
        {
            EncryptionParameters parms = new EncryptionParameters(SchemeType.BFV);
            ulong polyModulusDegree = 4096;
            parms.PolyModulusDegree = polyModulusDegree;
            parms.CoeffModulus = CoeffModulus.BFVDefault(polyModulusDegree);
            parms.PlainModulus = new Modulus(1024);
            return parms;
        }

        public Ciphertext Encrypt(int value, PublicKey publicKey)
        {
            using Encryptor encryptor = new Encryptor(Context, publicKey);
            using Plaintext xPlain = new Plaintext(value.ToString());
            Ciphertext xEncrypted = new Ciphertext();
            encryptor.Encrypt(xPlain, xEncrypted);
            return xEncrypted;
        }

        public Ciphertext Encrypt(int value)
        {
            using Encryptor encryptor = new Encryptor(Context, PublicKey);
            using Plaintext xPlain = new Plaintext(value.ToString());
            Ciphertext xEncrypted = new Ciphertext();
            encryptor.Encrypt(xPlain, xEncrypted);
            return xEncrypted;
        }

        public Ciphertext Encrypt(IEnumerable<ulong> values)
        {
            using Encryptor encryptor = new Encryptor(Context, PublicKey);
            using Plaintext xPlain = new Plaintext(values);
            Ciphertext xEncrypted = new Ciphertext();
            encryptor.Encrypt(xPlain, xEncrypted);
            return xEncrypted;
        }

        public Ciphertext Encrypt(IEnumerable<ulong> values, PublicKey publicKey)
        {
            using Encryptor encryptor = new Encryptor(Context, publicKey);
            using Plaintext xPlain = new Plaintext(values);
            Ciphertext xEncrypted = new Ciphertext();
            encryptor.Encrypt(xPlain, xEncrypted);
            return xEncrypted;
        }

        public int Decrypt(Ciphertext cipher)
        {
            using Decryptor decryptor = new Decryptor(Context, secretKey);
            using Plaintext xDecrypted = new Plaintext();
            decryptor.Decrypt(cipher, xDecrypted);
            int.TryParse(xDecrypted.ToString(), System.Globalization.NumberStyles.HexNumber, null, out int result);
            return result;
        }

        public Ciphertext AddCiphers(List<Ciphertext> values)
        {
            using Evaluator evaluator = new Evaluator(Context);
            Ciphertext result = new Ciphertext();
            evaluator.AddMany(values, result);
            return result;
        }
    }
}
