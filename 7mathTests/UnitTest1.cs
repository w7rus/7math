
using _7math;

namespace _7mathTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }
    
    public static float ToRad(float deg)
    {
        return (float)(Math.PI / 180) * deg;
    }
    
    [Test]
    public void Test1()
    {
        // var test = new Vector3()
        //     .Copy(Vector3.UnitX)
        //     .ApplyEuler(new Euler(z: ToRad(90), order: AxisOrder.ZYX));
        //
        // TestContext.Out.WriteLine(test.ToString());

        Vector3 SubV3(Vector3 v1, Vector3 v2) => v1.Subtract(v2);

        var v0 = new Vector3(1, 1, 1);
        var v1 = new Vector3(1, 1, 1);

        // var v2 = v1.Subtract(v0);
        var v2 = SubV3(v1, v0);
        
        TestContext.Out.WriteLine(v0.ToString());
        TestContext.Out.WriteLine(v1.ToString());
        TestContext.Out.WriteLine(v2.ToString());
    }
}