using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Karting.Camera;
using Karting.Car;
using System.Collections;

public class CameraFollowTests
{
    private GameObject cameraObject;
    private CameraFollow cameraFollow;
    private GameObject vehicleObject;
    private CarController3Mock carController3Mock;
    private GameObject focusPoint;

    [SetUp]
    public void SetUp()
    {
        // Create and set up the camera object
        cameraObject = new GameObject();
        cameraFollow = cameraObject.AddComponent<CameraFollow>();

        // Create and set up the vehicle object with the mock CarController3
        vehicleObject = new GameObject();
        carController3Mock = vehicleObject.AddComponent<CarController3Mock>();

        // Create and set up the focus point
        focusPoint = new GameObject();
        focusPoint.transform.parent = vehicleObject.transform;
        focusPoint.name = "focus";

        // Set up camera follow properties
        cameraFollow.attachedVehicle = vehicleObject;

        // Call Start manually since it's not called automatically in tests
        cameraFollow.Start();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(cameraObject);
        Object.DestroyImmediate(vehicleObject);
    }

    [Test]
    public void CycleCamera_ShouldUpdateLocationIndicator()
    {
        cameraFollow.CycleCamera();
        Assert.AreEqual(1, cameraFollow.locationIndicator);

        cameraFollow.CycleCamera();
        Assert.AreEqual(2, cameraFollow.locationIndicator);

        cameraFollow.CycleCamera();
        Assert.AreEqual(3, cameraFollow.locationIndicator);

        cameraFollow.CycleCamera();
        Assert.AreEqual(0, cameraFollow.locationIndicator);
    }

    private class CarController3Mock : CarController3
    {
        private float gforce = 0f;

        public override float GetGforce()
        {
            return gforce;
        }

        public void SetGforce(float value)
        {
            gforce = value;
        }
    }
}
