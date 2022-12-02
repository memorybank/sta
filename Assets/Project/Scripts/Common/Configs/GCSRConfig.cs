using UnityEngine;

namespace Playa.Config
{
	[CreateAssetMenu(fileName = "GCStreamingSpeechRecognitionConfig", menuName = "Playa/Config", order = 51)]
	public class GCSRConfig : ScriptableObject
	{
		[Range(0, 10)]
		public int maxAlternatives;

		public bool googleCredentialLoadFromResources;

		public string googleCredentialFilePath;

		public string googleCredentialJson;

		public bool interimResults;

		public bool enableAutomaticPunctuation;

		public bool enableWordTimeOffsets;

		public GCSRConfig()
		{
			maxAlternatives = 1;
			interimResults = true;
			enableAutomaticPunctuation = true;
			enableWordTimeOffsets = true;
			googleCredentialLoadFromResources = true;
			googleCredentialFilePath = string.Empty;
			googleCredentialJson = string.Empty;
		}
	}
}