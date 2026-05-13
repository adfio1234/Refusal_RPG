using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class WavyTMPText : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float waveHeight = 15f;
    [SerializeField] private float waveSpeed = 5f;
    [SerializeField] private float waveFrequency = 0.7f;

    private TMP_Text textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        textMesh.ForceMeshUpdate();

        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float waveOffset = Mathf.Sin(Time.time * waveSpeed + i * waveFrequency) * waveHeight;
            Vector3 offset = new Vector3(0f, waveOffset, 0f);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}