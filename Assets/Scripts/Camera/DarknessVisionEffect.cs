using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class DarknessVisionEffect : MonoBehaviour
{
    private const int MaxEmitterCount = 64;
    private static readonly System.Collections.Generic.List<DarknessVisionEffect> ActiveEffects =
        new System.Collections.Generic.List<DarknessVisionEffect>();

    [Header("Darkness")]
    [Range(0f, 1f)] public float darkness = 1f;
    [Range(0f, 1f)] public float ambientVisibility = 0f;
    [Range(0.01f, 2f)] public float edgeSoftness = 0.35f;
    [SerializeField] private bool effectEnabled = true;

    [Header("Shader")]
    [SerializeField] private Shader darknessShader;

    private Material darknessMaterial;
    private Camera targetCamera;
    private readonly Vector4[] emitterData = new Vector4[MaxEmitterCount];

    private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
    private static readonly int DarknessId = Shader.PropertyToID("_Darkness");
    private static readonly int AmbientId = Shader.PropertyToID("_Ambient");
    private static readonly int EdgeSoftnessId = Shader.PropertyToID("_EdgeSoftness");
    private static readonly int AspectId = Shader.PropertyToID("_Aspect");
    private static readonly int EmitterCountId = Shader.PropertyToID("_EmitterCount");
    private static readonly int EmittersId = Shader.PropertyToID("_Emitters");

    private void OnEnable()
    {
        targetCamera = GetComponent<Camera>();
        EnsureMaterial();

        if (!ActiveEffects.Contains(this))
        {
            ActiveEffects.Add(this);
        }
    }

    private void OnDisable()
    {
        ActiveEffects.Remove(this);

        if (darknessMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(darknessMaterial);
            }
            else
            {
                DestroyImmediate(darknessMaterial);
            }

            darknessMaterial = null;
        }
    }

    public static void SetActiveForAll(bool isActive)
    {
        for (int i = 0; i < ActiveEffects.Count; i++)
        {
            if (ActiveEffects[i] != null)
            {
                ActiveEffects[i].effectEnabled = isActive;
            }
        }
    }

    private void EnsureMaterial()
    {
        if (darknessShader == null)
        {
            darknessShader = Shader.Find("Hidden/DarknessVision");
        }

        if (darknessShader == null)
        {
            return;
        }

        if (darknessMaterial == null || darknessMaterial.shader != darknessShader)
        {
            if (darknessMaterial != null)
            {
                DestroyImmediate(darknessMaterial);
            }

            darknessMaterial = new Material(darknessShader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (!effectEnabled)
        {
            Graphics.Blit(source, destination);
            return;
        }

        EnsureMaterial();
        if (darknessMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        var emitters = VisionEmitter.ActiveEmitters;
        int emitterCount = 0;

        for (int i = 0; i < emitters.Count && emitterCount < MaxEmitterCount; i++)
        {
            VisionEmitter emitter = emitters[i];
            if (emitter == null || !emitter.isActiveAndEnabled || emitter.radius <= 0f || emitter.intensity <= 0f)
            {
                continue;
            }

            Vector3 emitterWorldPos = emitter.transform.position + emitter.offset;
            Vector3 emitterViewportPos = targetCamera.WorldToViewportPoint(emitterWorldPos);
            if (emitterViewportPos.z <= 0f)
            {
                continue;
            }

            Vector3 upSampleViewportPos = targetCamera.WorldToViewportPoint(emitterWorldPos + targetCamera.transform.up * emitter.radius);
            float viewportRadius = Mathf.Abs(upSampleViewportPos.y - emitterViewportPos.y);
            if (viewportRadius <= 0.00001f)
            {
                continue;
            }

            emitterData[emitterCount] = new Vector4(
                emitterViewportPos.x,
                emitterViewportPos.y,
                viewportRadius,
                emitter.intensity
            );

            emitterCount++;
        }

        darknessMaterial.SetFloat(DarknessId, darkness);
        darknessMaterial.SetFloat(AmbientId, ambientVisibility);
        darknessMaterial.SetFloat(EdgeSoftnessId, edgeSoftness);
        darknessMaterial.SetFloat(AspectId, (float)source.width / source.height);
        darknessMaterial.SetInt(EmitterCountId, emitterCount);
        darknessMaterial.SetVectorArray(EmittersId, emitterData);
        darknessMaterial.SetTexture(MainTexId, source);

        Graphics.Blit(source, destination, darknessMaterial);
    }
}
