using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class CameraScript : Node3D
{
    public Camera3D MainCamera;

    private float cameraZoom = 10;
    private float cameraZoomTo = 10;
    private int cameraZoomStrenght = 5;
    private int cameraZoomMax = 20;
    private int cameraZoomMin = -5;
    private float smoothTime = 0.1F;

    private float cameraMoveSpeed = 0.5f;
    private Vector2 mousePosition;
    private int winWidth;
    private int winHeight;
    public override void _Ready()
	{
        MainCamera = GetNode<Camera3D>("MainCamera");
        winWidth = (int)GetViewport().GetVisibleRect().Size.X;
        winHeight = (int)GetViewport().GetVisibleRect().Size.Y;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        #region CameraZoom
        
        cameraZoom = Mathf.Lerp(cameraZoom, cameraZoomTo, smoothTime);
        MainCamera.Position = Vector3.Right * MainCamera.Position.X + Vector3.Up * MainCamera.Position.Y + Vector3.Back * cameraZoom;

        if (Input.IsActionJustReleased("ScrollUp"))
        {
            cameraZoomTo = Mathf.Clamp(cameraZoomTo - cameraZoomStrenght, cameraZoomMin, cameraZoomMax);
		}

        else if (Input.IsActionJustReleased("ScrollDown"))
        {
            cameraZoomTo = Mathf.Clamp(cameraZoomTo + cameraZoomStrenght, cameraZoomMin, cameraZoomMax);
        }

        #endregion

        #region CameraKeyMovement
        mousePosition = GetViewport().GetMousePosition();
        if (Input.IsActionPressed("Left") || mousePosition.X <= winWidth * 0.02)
        {
            MainCamera.Translate(Vector3.Left * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("Right") || mousePosition.X >= winWidth * 0.98)
        {
            MainCamera.Translate(Vector3.Right * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("Up") || mousePosition.Y <= winHeight * 0.02)
        {
            MainCamera.Translate(Vector3.Up * cameraMoveSpeed);
        }
        if (Input.IsActionPressed("Down") || mousePosition.Y >= winHeight * 0.98)
        {
            MainCamera.Translate(Vector3.Down * cameraMoveSpeed);
        }
        #endregion
    }
}
