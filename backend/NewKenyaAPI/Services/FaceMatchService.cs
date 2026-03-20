using System.Security.Cryptography;

namespace NewKenyaAPI.Services
{
    public class FaceMatchService
    {
        private const decimal DefaultThreshold = 0.75m;

        public async Task<(decimal Score, bool Passed)> CompareAsync(IFormFile nidaDocument, IFormFile selfieDocument)
        {
            // Placeholder pipeline hook: replace with external AI face-match provider integration.
            var nidaFingerprint = await ComputeFingerprintAsync(nidaDocument);
            var selfieFingerprint = await ComputeFingerprintAsync(selfieDocument);

            var matchingCharacters = 0;
            var maxLength = Math.Min(nidaFingerprint.Length, selfieFingerprint.Length);
            for (var i = 0; i < maxLength; i++)
            {
                if (nidaFingerprint[i] == selfieFingerprint[i])
                {
                    matchingCharacters += 1;
                }
            }

            var score = maxLength == 0
                ? 0m
                : Math.Round((decimal)matchingCharacters / maxLength, 4, MidpointRounding.AwayFromZero);

            return (score, score >= DefaultThreshold);
        }

        private static async Task<string> ComputeFingerprintAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            var hashBytes = await SHA256.HashDataAsync(stream);
            return Convert.ToHexString(hashBytes);
        }
    }
}
