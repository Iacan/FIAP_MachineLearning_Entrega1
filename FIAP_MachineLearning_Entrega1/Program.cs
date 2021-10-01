using System;
using Microsoft.ProjectOxford.Face;
using System.Threading.Tasks;

namespace FIAP_MachineLearning_Entrega1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var image = "https://img.freepik.com/fotos-gratis/3d-rendem-de-uma-mesa-de-madeira-com-uma-imagem-defocussed-de-um-barco-em-um-lago_1048-3432.jpg?size=626&ext=jpg";
            var image = "https://www.lojaadcos.com.br/belezacomsaude/app/uploads/2018/10/00.protecao-solar.png";

            IFaceServiceClient faceServiceClient = new FaceServiceClient("a545b2f7035043e39c953b851f23e7bf",
                "https://fiap-facenovo.cognitiveservices.azure.com/face/v1.0");
            var detectedFaces = await faceServiceClient.DetectAsync(image);
            foreach (var detectedFace in detectedFaces)
            {
                Console.WriteLine($"{detectedFace.FaceId}");
            }

            if (detectedFaces.Length > 0)
            {
                var faceAttributes = new[] { FaceAttributeType.Emotion, FaceAttributeType.Age };

                var detectedFacesAtributos = await faceServiceClient.DetectAsync(image,
                    returnFaceAttributes: faceAttributes);

                foreach (var detectedFace in detectedFacesAtributos)
                {
                    Console.WriteLine($"{detectedFace.FaceId}");
                    Console.WriteLine($"Age = {detectedFace.FaceAttributes.Age}, Happiness = {detectedFace.FaceAttributes.Emotion.Happiness}");

                }

                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Não contém nenhum rosto!!!!");
                Console.ReadLine();
            }
        }
    }
}
