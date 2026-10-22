using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class CarControlTests
{
    private GameObject car;
    private CarControl control;
    private Rigidbody body;
    private WheelControl[] wheels;

    [SetUp]
    public void SetUp()
    {
        car = new GameObject("Throttle test car");
        car.SetActive(false);
        body = car.AddComponent<Rigidbody>();
        body.useGravity = false;
        control = car.AddComponent<CarControl>();
        control.enabled = false;
        wheels = new WheelControl[2];
        for (int i = 0; i < wheels.Length; i++)
        {
            GameObject wheelObject = new GameObject("Wheel");
            wheelObject.transform.SetParent(car.transform);
            WheelCollider collider = wheelObject.AddComponent<WheelCollider>();
            wheels[i] = wheelObject.AddComponent<WheelControl>();
            wheels[i].enabled = false;
            wheels[i].WheelCollider = collider;
            wheels[i].motorized = i == 0;
        }
        SetField("rigidBody", body);
        SetField("wheels", wheels);
        car.SetActive(true);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(car);
    }

    [TestCase(-0.5f, 10f)]
    [TestCase(-0.25f, 10f)]
    [TestCase(-0.5f, 0f)]
    [TestCase(-0.25f, 0f)]
    [TestCase(-0.5f, -10f)]
    [TestCase(-0.25f, -10f)]
    public void NegativeThrottleAppliesProportionalBrakesWithoutReverse(float throttle, float speed)
    {
        SetField("throttle", throttle);
        body.linearVelocity = Vector3.forward * speed;
        wheels[0].WheelCollider.motorTorque = 500f;

        Invoke("FixedUpdate");

        foreach (WheelControl wheel in wheels)
        {
            Assert.That(wheel.WheelCollider.motorTorque, Is.Zero);
            Assert.That(wheel.WheelCollider.brakeTorque,
                Is.EqualTo(-throttle * control.brakeTorque).Within(0.001f));
        }
    }

    [TestCase(0f)]
    [TestCase(0.5f)]
    [TestCase(1f)]
    public void NonnegativeThrottleReleasesBrakesAndPreservesForwardDrive(float throttle)
    {
        SetField("throttle", -0.5f);
        Invoke("FixedUpdate");
        SetField("throttle", throttle);

        Invoke("FixedUpdate");

        Assert.That(wheels[0].WheelCollider.motorTorque,
            Is.EqualTo(throttle * control.motorTorque).Within(0.001f));
        Assert.That(wheels[1].WheelCollider.motorTorque, Is.Zero);
        foreach (WheelControl wheel in wheels)
        {
            Assert.That(wheel.WheelCollider.brakeTorque, Is.Zero);
        }
    }

    [TestCase(-0.5f, 0f, 1f)]
    [TestCase(-0.25f, 0f, 0.5f)]
    [TestCase(0f, 0f, 0f)]
    [TestCase(0.5f, 0.5f, 0f)]
    [TestCase(1f, 1f, 0f)]
    public void ThrottleImagesDisplaySeparateNormalizedRanges(float throttle, float positiveFill, float negativeFill)
    {
        Image positive = CreateImage("ThrottleFrontPositive");
        Image negative = CreateImage("ThrottleFrontNegative");
        SetField("throttleBarImage", positive);
        SetField("negativeThrottleBarImage", negative);
        SetField("throttle", throttle);

        Invoke("Update");

        Assert.That(positive.fillAmount, Is.EqualTo(positiveFill).Within(0.001f));
        Assert.That(negative.fillAmount, Is.EqualTo(negativeFill).Within(0.001f));
    }

    private Image CreateImage(string name)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(car.transform);
        Image image = imageObject.GetComponent<Image>();
        image.type = Image.Type.Filled;
        return image;
    }

    private void SetField(string name, object value)
    {
        typeof(CarControl).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(control, value);
    }

    private void Invoke(string name)
    {
        typeof(CarControl).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(control, null);
    }
}
