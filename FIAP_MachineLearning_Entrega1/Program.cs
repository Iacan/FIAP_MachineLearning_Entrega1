using System;
using Microsoft.ProjectOxford.Face;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;

namespace FIAP_MachineLearning_Entrega1
{
    class Program
    {
        // Specify the features to return  
        private static readonly List<VisualFeatureTypes> features =
            new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
            VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags
        };

        static async Task Main(string[] args)
        {
            string image = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "FotoExemplos\\rostoperfil.png");
            //string image = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "FotoExemplos\\Paisagem.jpg");

            //Analisa se tem alguma pessoa na foto
            var flgHuman = Analyze(image).Result;

            if (flgHuman)
            {
                Console.WriteLine();
                Console.WriteLine("Tem 1 ou mais pessoas na foto!!!!");

                //Identificação de Face
                var subscriptionKeyFace = "a545b2f7035043e39c953b851f23e7bf";

                IFaceServiceClient faceServiceClient = new FaceServiceClient(subscriptionKeyFace,
                        "https://fiap-facenovo.cognitiveservices.azure.com/face/v1.0");
                
                using (Stream imageFileStream = File.OpenRead(image))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream,
                        true,
                        true,
                        new FaceAttributeType[] {
                    FaceAttributeType.Gender,
                    FaceAttributeType.Age,
                    FaceAttributeType.Emotion
                        });
                    var test =  faces;

                    //Verifica se existe faces na foto 
                    if (test.Length > 0)
                    {
                        foreach (var detectedFace in test)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"{detectedFace.FaceId}");
                            Console.WriteLine($"Idade = {detectedFace.FaceAttributes.Age}, Felicidade = {detectedFace.FaceAttributes.Emotion.Happiness}");
                        }

                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("Não contém nenhum rosto!!!!");
                        Console.ReadLine();
                    }
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Não tem pessoas na foto!!!");
                Console.ReadLine();
            }
        }

        static async Task<bool> Analyze(string image)
        {
            string key = "1ab69f70161248e29e02a70568500dee";
            HttpClient c = new HttpClient();
            c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            string reqParams = "visualFeatures=Categories,Description&language=en";
            string uriBase = "https://fiap-computer-vision-novo.cognitiveservices.azure.com/vision/v1.0/analyze";
            string uri = uriBase + "?" + reqParams;

            FileStream fileStream = new FileStream(image, FileMode.Open, FileAccess.Read);
            BinaryReader bReader = new BinaryReader(fileStream);
            byte[] byteData = bReader.ReadBytes((int)fileStream.Length);
            ByteArrayContent content = new ByteArrayContent(byteData);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            HttpResponseMessage response = await c.PostAsync(uri, content);            
            string contentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("\nAnalysis:\n");
            Console.WriteLine(contentString);
            
            var flgHuman = contentString.Contains("woman") || contentString.Contains("men") ? true : false;

            return flgHuman;
        } 
    }
}
