using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Aligner : MonoBehaviour
{
    public enum AlignMode
    {
        INSIDE, OUTSIDE
    }

    [SerializeField]
    private GameObject reference;
    public bool alignLeft;
    public bool alignRight;
    public bool alignCenterX;
    public bool alignTop;
    public bool alignBottom;
    public bool alignCenterY;
    public AlignMode alignMode;
    public bool includeMasked = true;
    public bool preserveAspectRatio = true;
    public bool checkEveryFrame;

    public GameObject Reference
    {
        get
        {
            return reference;
        }
        set
        {
            reference = value;
            Align();
        }
    }

    public bool IsInitialized { get; private set; }

    private bool lastAlignLeft;
    private bool lastAlignRight;
    private bool lastAlignCenterX;
    private bool lastAlignTop;
    private bool lastAlignBottom;
    private bool lastAlignCenterY;

    private Vector4 referenceLimits;
    private Vector2 halfDimension;
    private Vector2 referenceHalfDimension;

    private void Awake()
    {
        gameObject.SetActive(false);
        Align();
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorApplication.update += ManageAlign;
        }
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) 
        {
            EditorApplication.update -= ManageAlign;
        }
#endif
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            ManageAlign();
        }
    }

    private void ManageAlign()
    {
        if ((alignLeft && !lastAlignLeft) || (alignRight && !lastAlignRight))
        {
            alignCenterX = false;
        }
        if (alignCenterX)
        {
            alignLeft = false;
            alignRight = false;
        }
        if ((alignTop && !lastAlignTop) || (alignBottom && !alignBottom))
        {
            alignCenterY = false;
        }
        if (alignCenterY)
        {
            alignTop = false;
            alignBottom = false;
        }

        if (checkEveryFrame
            || alignLeft != lastAlignLeft
            || alignRight != lastAlignRight
            || alignCenterX != lastAlignCenterX
            || alignTop != lastAlignTop
            || alignBottom != lastAlignBottom
            || alignCenterY != lastAlignCenterY)
        {
            try
            {
                Align();
            }
            catch
            {

            }
        }

        lastAlignLeft = alignLeft;
        lastAlignRight = alignRight;
        lastAlignCenterX = alignCenterX;
        lastAlignTop = alignTop;
        lastAlignBottom = alignBottom;
        lastAlignCenterY = alignCenterY;
    }

    public void Align()
    {
        if (reference == null)
        {
            return;
        }

        referenceLimits
            = TransformUtils.GetLimitsOfTransform(
                reference.transform,
                includeMasked
            );
        halfDimension
            = TransformUtils.GetSizeOfTransform(
                transform,
                includeMasked)
            / 2;
        referenceHalfDimension
            = TransformUtils.GetSizeOfTransform(
                reference.transform,
                includeMasked)
            / 2;

        if (referenceLimits.Equals(Vector4.zero)
            || halfDimension.Equals(Vector2.zero)
            || referenceHalfDimension.Equals(Vector2.zero)) {
            return;
        }

        if (alignLeft && alignRight && alignTop && alignBottom)
        {
            if (preserveAspectRatio)
            {
                float aspectRatio = halfDimension.x
                    / halfDimension.y;
                float aspectRatioReference = referenceHalfDimension.x
                    / referenceHalfDimension.y;

                if ((halfDimension.y <= halfDimension.x
                    && aspectRatio <= aspectRatioReference)
                    || (halfDimension.y >= halfDimension.x
                    && aspectRatio <= aspectRatioReference))
                {
                    if (alignMode == AlignMode.INSIDE)
                    {
                        AlignCenterX();
                        AlignTopAndBottom();
                    }
                    else if (alignMode == AlignMode.OUTSIDE)
                    {
                        AlignCenterY();
                        AlignLeftAndRight();
                    }
                }
                else if ((halfDimension.y >= halfDimension.x
                    && aspectRatio >= aspectRatioReference)
                    || (halfDimension.y <= halfDimension.x
                    && aspectRatio >= aspectRatioReference))
                {
                    if (alignMode == AlignMode.INSIDE)
                    {
                        AlignCenterY();
                        AlignLeftAndRight();
                    }
                    else if (alignMode == AlignMode.OUTSIDE)
                    {
                        AlignCenterX();
                        AlignTopAndBottom();
                    }
                }
            }
            else
            {
                AlignTopAndBottom();
                AlignLeftAndRight();
            }
        }
        else
        {
            if (alignCenterX)
            {
                AlignCenterX();
            }
            else
            {
                if (alignLeft && alignRight)
                {
                    AlignLeftAndRight();
                }
                else
                {
                    if (alignRight)
                    {
                        AlignRight();
                    }
                    if (alignLeft)
                    {
                        AlignLeft();
                    }
                }
            }

            if (alignCenterY)
            {
                AlignCenterY();
            }
            else
            {
                if (alignTop && alignBottom)
                {
                    AlignTopAndBottom();
                }
                else
                {
                    if (alignTop)
                    {
                        AlignTop();
                    }
                    if (alignBottom)
                    {
                        AlignBottom();
                    }
                }
            }
        }

        IsInitialized = true;
    }

    private void AlignRight()
    {
        float maxX = referenceLimits.w;
        if (alignMode == AlignMode.OUTSIDE)
        {
            maxX -= referenceHalfDimension.x * 2;
        }
        transform.position = new Vector3(
                maxX
                - halfDimension.x,
                transform.position.y,
                transform.position.z
            );
    }

    private void AlignLeft()
    {
        float minX = referenceLimits.z;
        if (alignMode == AlignMode.OUTSIDE)
        {
            minX += referenceHalfDimension.x * 2;
        }
        transform.position = new Vector3(
                minX
                + halfDimension.x,
                transform.position.y,
                transform.position.z
            );
    }

    private void AlignCenterX()
    {
        float centerX = (referenceLimits.z + referenceLimits.w) / 2f;
        transform.position = new Vector3(
                centerX,
                transform.position.y,
                transform.position.z
            );
    }

    private void AlignLeftAndRight()
    {
        float maxX = referenceLimits.w;
        float minX = referenceLimits.z;
        float width = maxX - minX;
        float scaleX = width / (halfDimension.x * 2);
        float scaleY = 1;
        if (preserveAspectRatio)
        {
            scaleY = scaleX;
        }
        transform.localScale = new Vector3(
            scaleX * transform.localScale.x,
            scaleY * transform.localScale.y,
            transform.localScale.z
        );
        AlignCenterX();
    }

    private void AlignTop()
    {
        float maxY = referenceLimits.x;
        if (alignMode == AlignMode.OUTSIDE)
        {
            maxY -= referenceHalfDimension.y * 2;
        }
        transform.position = new Vector3(
                transform.position.x,
                maxY
                - halfDimension.y,
                transform.position.z
            );
    }

    private void AlignBottom()
    {
        float minY = referenceLimits.y;
        if (alignMode == AlignMode.OUTSIDE)
        {
            minY += referenceHalfDimension.y * 2;
        }
        transform.position = new Vector3(
                transform.position.x,
                minY
                + halfDimension.y,
                transform.position.z
            );
    }

    private void AlignCenterY()
    {
        float centerY = (referenceLimits.x + referenceLimits.y) / 2f;
        transform.position = new Vector3(
                transform.position.x,
                centerY,
                transform.position.z
            );
    }

    private void AlignTopAndBottom()
    {
        float maxY = referenceLimits.x;
        float minY = referenceLimits.y;
        float height = maxY - minY;
        float scaleY = height / (halfDimension.y * 2);
        float scaleX = 1;
        if (preserveAspectRatio)
        {
            scaleX = scaleY;
        }
        transform.localScale = new Vector3(
            scaleX * transform.localScale.x,
            scaleY * transform.localScale.y,
            transform.localScale.z
        );
        AlignCenterY();
    }
}
