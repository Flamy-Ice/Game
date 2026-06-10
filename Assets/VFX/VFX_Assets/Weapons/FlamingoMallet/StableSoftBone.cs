using UnityEngine;

public class StableSoftBone : MonoBehaviour
{
    [Header("Bones Setup")]
    [Tooltip("Dodaj koœci po kolei: od bazy/g³owy do koñca.")]
    public Transform[] bones;

    [Header("Physics Settings")]
    [Range(0f, 1f)] public float drag = 0.15f;   // Opór powietrza (im wiêkszy, tym mniej "macha")
    public float gravity = -9.81f;               // Grawitacja ci¹gn¹ca w dó³

    [Range(0f, 500f)]
    public float stiffness = 50f;                // SI£A POWROTU: Im wiêksza, tym szybciej g³owa wraca na swoje miejsce

    private Vector3[] currentPositions;
    private Vector3[] previousPositions;
    private float[] boneLengths;
    private Quaternion[] localRestRotations;

    void Start()
    {
        if (bones == null || bones.Length < 2)
        {
            Debug.LogWarning("Przypisz co najmniej 2 koœci w tablicy Bones!");
            return;
        }

        currentPositions = new Vector3[bones.Length];
        previousPositions = new Vector3[bones.Length];
        boneLengths = new float[bones.Length - 1];
        localRestRotations = new Quaternion[bones.Length];

        for (int i = 0; i < bones.Length; i++)
        {
            currentPositions[i] = bones[i].position;
            previousPositions[i] = bones[i].position;
            localRestRotations[i] = bones[i].localRotation;

            if (i > 0)
            {
                boneLengths[i - 1] = Vector3.Distance(bones[i].position, bones[i - 1].position);
            }
        }
    }

    void LateUpdate()
    {
        if (bones == null || bones.Length < 2 || Time.deltaTime == 0f) return;

        // KROK 1: Resetujemy rotacje, aby Unity obliczy³o "idealn¹" pozycjê koœci w tej klatce
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].localRotation = localRestRotations[i];
        }

        // KROK 2: Kotwiczymy pierwsz¹ koœæ
        currentPositions[0] = bones[0].position;

        // KROK 3: Fizyka (bezw³adnoœæ + grawitacja + NOWA si³a powrotu kszta³tu)
        for (int i = 1; i < bones.Length; i++)
        {
            // Pobieramy idealn¹ pozycjê koœci (wynikaj¹c¹ z oryginalnego kszta³tu modelu)
            Vector3 idealPos = bones[i].position;

            Vector3 velocity = (currentPositions[i] - previousPositions[i]) * (1f - drag);
            previousPositions[i] = currentPositions[i];

            // Obliczamy si³ê sprê¿ystoœci, która ci¹gnie koœæ do jej oryginalnego miejsca
            Vector3 springForce = (idealPos - currentPositions[i]) * stiffness;
            Vector3 gravityForce = Vector3.up * gravity;

            Vector3 acceleration = springForce + gravityForce;

            // Aktualizacja pozycji punktu
            currentPositions[i] += velocity + (acceleration * Time.deltaTime * Time.deltaTime);
        }

        // KROK 4: Wiêzy odleg³oœci (Gwarancja braku rozci¹gania siatki)
        for (int iteration = 0; iteration < 4; iteration++)
        {
            for (int i = 1; i < bones.Length; i++)
            {
                Vector3 dir = currentPositions[i] - currentPositions[i - 1];
                float currentLength = dir.magnitude;
                if (currentLength > 0.001f)
                {
                    Vector3 targetDir = dir / currentLength;
                    float diff = boneLengths[i - 1] - currentLength;
                    currentPositions[i] += targetDir * diff;
                }
            }
        }

        // KROK 5: Finalne obracanie koœci w stronê obliczonych punktów fizycznych
        for (int i = 0; i < bones.Length - 1; i++)
        {
            Vector3 naturalChildPos = bones[i + 1].position;
            Vector3 targetChildPos = currentPositions[i + 1];

            Vector3 currentDir = naturalChildPos - bones[i].position;
            Vector3 targetDir = targetChildPos - bones[i].position;

            if (currentDir.sqrMagnitude > 0.0001f && targetDir.sqrMagnitude > 0.0001f)
            {
                Quaternion lookRot = Quaternion.FromToRotation(currentDir.normalized, targetDir.normalized);
                bones[i].rotation = lookRot * bones[i].rotation;
            }
        }
    }
}