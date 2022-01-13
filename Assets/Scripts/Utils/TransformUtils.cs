using UnityEngine;

public class TransformUtils
{
	/// <summary>
	/// Calculates the size of a transform by calculating its rendering limits
	/// and subtracting them on both X and Y axis.
	/// </summary>
	public static Vector2 GetSizeOfTransform(
		Transform transform,
		bool includeMasked = true,
		bool includeInactive = false)
	{
		Vector4 limits = GetLimitsOfTransform(
			transform,
			includeMasked,
			includeInactive
		);
		return new Vector2(limits.w - limits.z, limits.x - limits.y);
	}

	/// <summary>
	/// Caluclates the rendering limits of a transform on X and Y axis.
	/// By limits we understand the points that the object gets rendered
	/// between. The function takes all <see cref="SpriteRenderer"/>,
	/// all <see cref="MeshRenderer"/> and all <see cref="Camera"/> components
	/// from the <paramref name="transform"/> and its children and finds the
	/// starting and ending rendering points on both X and Y axis.
	/// </summary>
	public static Vector4 GetLimitsOfTransform(
		Transform transform,
		bool includeMasked = true,
		bool includeInactive = false)
	{
		Vector4 limits = Vector4.zero; // x - Up, y - down, z - left, w - right
		float possibleLimit;

		if (transform is RectTransform)
		{
			bool firstRectTransform = true;
			Vector3[] corners = new Vector3[4];
			foreach (RectTransform rectTransform
				in transform.GetComponentsInChildren<RectTransform>())
			{
				if (firstRectTransform)
				{
					firstRectTransform = false;
					rectTransform.GetWorldCorners(corners);
					limits.x = corners[1].y;
					limits.y = corners[0].y;
					limits.z = corners[0].x;
					limits.w = corners[2].x;
				}
				else
				{
					possibleLimit = corners[1].y;
					if (possibleLimit > limits.x)
					{
						limits.x = possibleLimit;
					}
					possibleLimit = corners[0].y;
					if (possibleLimit < limits.y)
					{
						limits.y = possibleLimit;
					}
					possibleLimit = corners[0].x;
					if (possibleLimit < limits.z)
					{
						limits.z = possibleLimit;
					}
					possibleLimit = corners[2].x;
					if (possibleLimit > limits.w)
					{
						limits.w = possibleLimit;
					}
				}
			}
			return limits;
		}

		bool firstRenderer = true;
		foreach (SpriteRenderer spriteRenderer
			in transform.GetComponentsInChildren<SpriteRenderer>())
		{
			if (!includeMasked
				&& spriteRenderer.maskInteraction
					!= SpriteMaskInteraction.None)
			{
				continue;
			}
			if (!includeInactive
				&& !spriteRenderer.gameObject.activeInHierarchy)
			{
				continue;
			}
			if (firstRenderer)
			{
				firstRenderer = false;
				limits.x = spriteRenderer.transform.position.y
					+ spriteRenderer.bounds.size.y / 2f;
				limits.y = spriteRenderer.transform.position.y
					- spriteRenderer.bounds.size.y / 2f;
				limits.z = spriteRenderer.transform.position.x
					- spriteRenderer.bounds.size.x / 2f;
				limits.w = spriteRenderer.transform.position.x
					+ spriteRenderer.bounds.size.x / 2f;
			}
			else
			{
				possibleLimit = spriteRenderer.transform.position.y
					+ spriteRenderer.bounds.size.y / 2f;
				if (possibleLimit > limits.x)
				{
					limits.x = possibleLimit;
				}
				possibleLimit = spriteRenderer.transform.position.y
					- spriteRenderer.bounds.size.y / 2f;
				if (possibleLimit < limits.y)
				{
					limits.y = possibleLimit;
				}
				possibleLimit = spriteRenderer.transform.position.x
					- spriteRenderer.bounds.size.x / 2f;
				if (possibleLimit < limits.z)
				{
					limits.z = possibleLimit;
				}
				possibleLimit = spriteRenderer.transform.position.x
					+ spriteRenderer.bounds.size.x / 2f;
				if (possibleLimit > limits.w)
				{
					limits.w = possibleLimit;
				}
			}
		}
		foreach (MeshRenderer meshRenderer
			in transform.GetComponentsInChildren<MeshRenderer>())
		{
			if (!meshRenderer.gameObject.activeInHierarchy)
			{
				continue;
			}
			if (firstRenderer)
			{
				firstRenderer = false;
				limits.x = meshRenderer.transform.position.y
					+ meshRenderer.bounds.size.y / 2f;
				limits.y = meshRenderer.transform.position.y
					- meshRenderer.bounds.size.y / 2f;
				limits.z = meshRenderer.transform.position.x
					- meshRenderer.bounds.size.x / 2f;
				limits.w = meshRenderer.transform.position.x
					+ meshRenderer.bounds.size.x / 2f;
			}
			else
			{
				possibleLimit = meshRenderer.transform.position.y
					+ meshRenderer.bounds.size.y / 2f;
				if (possibleLimit > limits.x)
				{
					limits.x = possibleLimit;
				}
				possibleLimit = meshRenderer.transform.position.y
					- meshRenderer.bounds.size.y / 2f;
				if (possibleLimit < limits.y)
				{
					limits.y = possibleLimit;
				}
				possibleLimit = meshRenderer.transform.position.x
					- meshRenderer.bounds.size.x / 2f;
				if (possibleLimit < limits.z)
				{
					limits.z = possibleLimit;
				}
				possibleLimit = meshRenderer.transform.position.x
					+ meshRenderer.bounds.size.x / 2f;
				if (possibleLimit > limits.w)
				{
					limits.w = possibleLimit;
				}
			}
		}
		Vector2 cameraSize;
		foreach (Camera camera
			in transform.GetComponentsInChildren<Camera>())
		{
			if (!camera.gameObject.activeInHierarchy)
			{
				continue;
			}
			cameraSize = CameraUtils.GetCameraSize(camera);
			if (firstRenderer)
			{
				firstRenderer = false;
				limits.x = camera.transform.position.y
					+ cameraSize.y / 2f;
				limits.y = camera.transform.position.y
					- cameraSize.y / 2f;
				limits.z = camera.transform.position.x
					- cameraSize.x / 2f;
				limits.w = camera.transform.position.x
					+ cameraSize.x / 2f;
			}
			else
			{
				possibleLimit = camera.transform.position.y
					+ cameraSize.y / 2f;
				if (possibleLimit > limits.x)
				{
					limits.x = possibleLimit;
				}
				possibleLimit = camera.transform.position.y
					- cameraSize.y / 2f;
				if (possibleLimit < limits.y)
				{
					limits.y = possibleLimit;
				}
				possibleLimit = camera.transform.position.x
					- cameraSize.x / 2f;
				if (possibleLimit < limits.z)
				{
					limits.z = possibleLimit;
				}
				possibleLimit = camera.transform.position.x
					+ cameraSize.x / 2f;
				if (possibleLimit > limits.w)
				{
					limits.w = possibleLimit;
				}
			}
		}
		return limits;
	}
}
