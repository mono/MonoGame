// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MonoGame.Tests.Framework
{
    [TestFixture]
    class Bounding
    {
        [Test]
        public void BoxContainsVector3Test()
        {
            var box = new BoundingBox(Vector3.Zero, Vector3.One);

            Assert.AreEqual(ContainmentType.Disjoint, box.Contains(-Vector3.One));
            Assert.AreEqual(ContainmentType.Disjoint, box.Contains(new Vector3(0.5f, 0.5f, -1f)));
            Assert.AreEqual(ContainmentType.Contains, box.Contains(Vector3.Zero));
            Assert.AreEqual(ContainmentType.Contains, box.Contains(new Vector3(0f, 0, 0.5f)));
            Assert.AreEqual(ContainmentType.Contains, box.Contains(new Vector3(0f, 0.5f, 0.5f)));
            Assert.AreEqual(ContainmentType.Contains, box.Contains(Vector3.One));
            Assert.AreEqual(ContainmentType.Contains, box.Contains(new Vector3(1f, 1, 0.5f)));
            Assert.AreEqual(ContainmentType.Contains, box.Contains(new Vector3(1f, 0.5f, 0.5f)));
            Assert.AreEqual(ContainmentType.Contains, box.Contains(new Vector3(0.5f, 0.5f, 0.5f)));
        }

        [Test]
        public void BoxContainsIdenticalBox()
        {
            var b1 = new BoundingBox(Vector3.Zero, Vector3.One);
            var b2 = new BoundingBox(Vector3.Zero, Vector3.One);

            Assert.AreEqual(ContainmentType.Contains, b1.Contains(b2));
        }

        [Test]
        public void BoundingSphereTests()
        {
            var zeroPoint = BoundingSphere.CreateFromPoints( new[] {Vector3.Zero} );
            Assert.AreEqual(new BoundingSphere(), zeroPoint);

            var onePoint = BoundingSphere.CreateFromPoints(new[] { Vector3.One });
            Assert.AreEqual(new BoundingSphere(Vector3.One, 0), onePoint);

            var twoPoint = BoundingSphere.CreateFromPoints(new[] { Vector3.Zero, Vector3.One });
            Assert.AreEqual(new BoundingSphere(new Vector3(0.5f, 0.5f, 0.5f), 0.8660254f), twoPoint);

            var threePoint = BoundingSphere.CreateFromPoints(new[] { new Vector3(0, 0, 0), new Vector3(-1, 0, 0), new Vector3(1, 1, 1) });
            Assert.That(new BoundingSphere(new Vector3(0, 0.5f, 0.5f), 1.224745f), Is.EqualTo(threePoint).Using(BoundingSphereComparer.Epsilon));

            var eightPointTestInput = new Vector3[]
            {
                new Vector3(54.58071f, 124.9063f, 56.0016f),
                new Vector3(54.52138f, 124.9063f, 56.13985f),
                new Vector3(54.52208f, 124.8235f, 56.14014f),
                new Vector3(54.5814f, 124.8235f, 56.0019f),
                new Vector3(1145.415f, 505.913f, -212.5173f),
                new Vector3(611.4731f, 505.9535f, 1031.893f),
                new Vector3(617.7462f, -239.7422f, 1034.584f),
                new Vector3(1151.687f, -239.7035f, -209.8246f)
            };
            var eightPoint = BoundingSphere.CreateFromPoints(eightPointTestInput);
            for (int i = 0; i < eightPointTestInput.Length; i++)
            {
                Assert.That(eightPoint.Contains(eightPointTestInput[i]) != ContainmentType.Disjoint);
            }

            Assert.Throws<ArgumentException>(() => BoundingSphere.CreateFromPoints(new Vector3[] {}));
        }
        public static System.Collections.Generic.IEnumerable<TestCaseData> FrustumSource
        {
            get
            {
                Matrix view, projection;
                BoundingFrustum frustum;

                // Frustum planes intersect with the box sides
                view = Matrix.CreateLookAt(new Vector3(0, 0, -0.2f), Vector3.Zero, Vector3.Up);
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 0.1f, 5);
                frustum = new BoundingFrustum(view * projection);

                yield return new TestCaseData(frustum, ContainmentType.Intersects);

                // Box is entirely inside the frustum
                view = Matrix.CreateLookAt(new Vector3(0.5f, 0.5f, -10), new Vector3(0.5f, 0.5f, 0), Vector3.Up);
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1, 50);
                frustum = new BoundingFrustum(view * projection);

                yield return new TestCaseData(frustum, ContainmentType.Intersects);

                view = Matrix.CreateLookAt(new Vector3(2f, 0.5f, 0), new Vector3(2, 0.5f, 1), Vector3.Up);
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1, 0.1f, 5);
                frustum = new BoundingFrustum(view * projection);

                yield return new TestCaseData(frustum, ContainmentType.Intersects);

                // Frustum is entirely inside the box
                view = Matrix.CreateLookAt(new Vector3(0.5f, 0.5f, 0.1f), new Vector3(0.5f, 0.5f, 0.3f), Vector3.Up);
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 0.1f, 0.2f);
                frustum = new BoundingFrustum(view * projection);

                yield return new TestCaseData(frustum, ContainmentType.Contains);

                // Camera is far away, looking at the box with farclip almost touching
                view = Matrix.CreateLookAt(new Vector3(0.5f, 0.5f, -100), new Vector3(0.5f, 0.5f, 0.5f), Vector3.Up);
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1, 100 - 0.01f);
                frustum = new BoundingFrustum(view * projection);

                yield return new TestCaseData(frustum, ContainmentType.Disjoint);

                // Left plane of frustum is almost touching corner of a box
                view = Matrix.CreateLookAt(new Vector3(1.1f, 1.1f, 0.5f), new Vector3(1.1f, 1.1f, 1f), Vector3.Normalize(new Vector3(-1, 1, 0)));
                projection = Matrix.CreatePerspectiveFieldOfView(MathF.Atan(0.5f), 1, 0.1f, 1.1f);
                frustum = new BoundingFrustum(view * projection);

                yield return new TestCaseData(frustum, ContainmentType.Disjoint);

                // Near plane of frustum offset by 45 deg is almost touching face of a box
                view = Matrix.CreateLookAt(new Vector3(0.5f, 0, 0.5f), new Vector3(1f, 0.5f, 0.5f), new Vector3(-1, 1, 0));
                var cos1 = MathF.Cos((float)(MathF.PI / 16.0));
                var cos2 = 1.0f / (2.0f * MathF.Cos((float)(5.0 * MathF.PI / 16.0)));
                projection = Matrix.CreatePerspectiveFieldOfView(MathF.PI / 8.0f, 1, cos1 * cos2, 12);
                frustum = new BoundingFrustum(view * projection);

                yield return new TestCaseData(frustum, ContainmentType.Disjoint);
            }
        }

        [Test]
        [TestCaseSource(nameof(FrustumSource))]
        public void BoundingBoxContainsPreciseFrustumTests(BoundingFrustum frustum, ContainmentType expectedValue)
        {
            BoundingBox box = new BoundingBox(Vector3.Zero, Vector3.One);
            var actualValue = box.ContainsPrecise(frustum);

            Assert.AreEqual(expectedValue, actualValue);
        }


        [Test]
        public void BoundingBoxContainsBoundingSphere()
        {
            var testSphere = new BoundingSphere(Vector3.Zero, 1);
            var testBox = new BoundingBox(-Vector3.One, Vector3.One);

            Assert.AreEqual(testBox.Contains(testSphere), ContainmentType.Contains);

            testSphere.Center -= Vector3.One;

            Assert.AreEqual(testBox.Contains(testSphere), ContainmentType.Intersects);

            testSphere.Center -= Vector3.One;

            Assert.AreEqual(testBox.Contains(testSphere), ContainmentType.Disjoint);
        }

        [Test]
        public void BoundingFrustumToBoundingBoxTests()
        {
            var view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1, 100);
            var testFrustum = new BoundingFrustum(view * projection);

            var bbox1 = new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1));
            Assert.That(testFrustum.Contains(bbox1), Is.EqualTo(ContainmentType.Contains));
            Assert.That(testFrustum.Intersects(bbox1), Is.True);

            var bbox2 = new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000));
            Assert.That(testFrustum.Contains(bbox2), Is.EqualTo(ContainmentType.Intersects));
            Assert.That(testFrustum.Intersects(bbox2), Is.True);

            var bbox3 = new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(0, 0, 0));
            Assert.That(testFrustum.Contains(bbox3), Is.EqualTo(ContainmentType.Intersects));
            Assert.That(testFrustum.Intersects(bbox3), Is.True);

            var bbox4 = new BoundingBox(new Vector3(-1000, -1000, -1000), new Vector3(-500, -500, -500));
            Assert.That(testFrustum.Contains(bbox4), Is.EqualTo(ContainmentType.Disjoint));
            Assert.That(testFrustum.Intersects(bbox4), Is.False);
        }

        [Test]
        public void BoundingFrustumToBoundingFrustumTests()
        {
            var view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1, 100);
            var testFrustum = new BoundingFrustum(view * projection);

            // Same frustum.
            Assert.That(testFrustum.Contains(testFrustum), Is.EqualTo(ContainmentType.Contains));
            Assert.That(testFrustum.Intersects(testFrustum), Is.True);

            var otherFrustum = new BoundingFrustum(Matrix.Identity);

            // Smaller frustum contained entirely inside.
            var view2 = Matrix.CreateLookAt(new Vector3(0, 0, 4), Vector3.Zero, Vector3.Up);
            var projection2 = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1, 50);
            otherFrustum.Matrix = view2 * projection2;

            Assert.That(testFrustum.Contains(otherFrustum), Is.EqualTo(ContainmentType.Contains));
            Assert.That(testFrustum.Intersects(otherFrustum), Is.True);

            // Same size frustum, pointing in the same direction and offset by a small amount.
            otherFrustum.Matrix = view2 * projection;

            Assert.That(testFrustum.Contains(otherFrustum), Is.EqualTo(ContainmentType.Intersects));
            Assert.That(testFrustum.Intersects(otherFrustum), Is.True);

            // Same size frustum, pointing in the opposite direction and not overlapping.
            var view3 = Matrix.CreateLookAt(new Vector3(0, 0, 6), new Vector3(0, 0, 7), Vector3.Up);
            otherFrustum.Matrix = view3 * projection;

            Assert.That(testFrustum.Contains(otherFrustum), Is.EqualTo(ContainmentType.Disjoint));
            Assert.That(testFrustum.Intersects(otherFrustum), Is.False);

            // Larger frustum, entirely containing test frustum.
            var view4 = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up);
            var projection4 = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1, 1000);
            otherFrustum.Matrix = view4 * projection4;

            Assert.That(testFrustum.Contains(otherFrustum), Is.EqualTo(ContainmentType.Intersects));
            Assert.That(testFrustum.Intersects(otherFrustum), Is.True);

            var bf =
                new BoundingFrustum(Matrix.CreateLookAt(new Vector3(0, 1, 1), new Vector3(0, 0, 0), Vector3.Up) *
                                    Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                        1.3f, 0.1f, 1000.0f));
            var ray = new Ray(new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0, 0));
            var ray2 = new Ray(new Vector3(0, 1.0f, 1.0f), new Vector3(0, 0, 0));
            var value = bf.Intersects(ray);
            var value2 = bf.Intersects(ray2);
            Assert.AreEqual(0.0f, value);
            Assert.AreEqual(null, value2);
        }

#if !XNA
        [Test]
        public void BoundingBoxDeconstruct()
        {
            BoundingBox boundingBox = new BoundingBox(new Vector3(255, 255, 255), new Vector3(0, 0, 0));

            Vector3 min, max;

            boundingBox.Deconstruct(out min, out max);

            Assert.AreEqual(min, boundingBox.Min);
            Assert.AreEqual(max, boundingBox.Max);
        }

        [Test]
        public void BoundingSphereDeconstruct()
        {
            BoundingSphere boundingSphere = new BoundingSphere(new Vector3(255, 255, 255), float.MaxValue);

            Vector3 center;
            float radius;

            boundingSphere.Deconstruct(out center, out radius);

            Assert.AreEqual(center, boundingSphere.Center);
            Assert.AreEqual(radius, boundingSphere.Radius);
        }
#endif
    }
}
