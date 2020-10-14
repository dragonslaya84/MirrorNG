﻿using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Mirror.Tests
{
    [Timeout(2000)]
    public class MultiplexTransportTest
    {
        #region SetUp

        private GameObject transportObj;
        private MultiplexTransport transport;

        private Transport transport1;
        private Transport transport2;

        IConnection conn1;
        IConnection conn2;

        [SetUp]
        public void Setup()
        {
            transportObj = new GameObject();

            transport = transportObj.AddComponent<MultiplexTransport>();

            // this gives warnings,  it is ok
            transport1 = Substitute.For<Transport>();
            transport2 = Substitute.For<Transport>();

            transport1.Supported.Returns(true);
            transport2.Supported.Returns(true);

            transport.transports = new[] { transport1, transport2 };
            conn1 = Substitute.For<IConnection>();
            conn2 = Substitute.For<IConnection>();

        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(transportObj);
        }
        #endregion

        [UnityTest]
        public IEnumerator AcceptTransport1() => UniTask.ToCoroutine(async () =>
        {
            transport1.AcceptAsync().Returns(UniTask.FromResult(conn1));

            Assert.That(await transport.AcceptAsync(), Is.SameAs(conn1));
        });

        [UnityTest]
        public IEnumerator AcceptTransport2() => UniTask.ToCoroutine(async () =>
        {
            transport2.AcceptAsync().Returns(UniTask.FromResult(conn1));
            // transport1 task never ends
            transport1.AcceptAsync().Returns(new UniTaskCompletionSource<IConnection>().Task);
            Assert.That(await transport.AcceptAsync(), Is.SameAs(conn1));
        });

        [UnityTest]
        public IEnumerator AcceptMultiple() => UniTask.ToCoroutine(async () =>
        {
            transport1.AcceptAsync().Returns(UniTask.FromResult(conn1), UniTask.FromResult(conn2));
            // transport2 task never ends
            transport2.AcceptAsync().Returns(new UniTaskCompletionSource<IConnection>().Task);
            Assert.That(await transport.AcceptAsync(), Is.SameAs(conn1));
            Assert.That(await transport.AcceptAsync(), Is.SameAs(conn2));
        });

        [UnityTest]
        public IEnumerator AcceptUntilAllGone() => UniTask.ToCoroutine(async () =>
        {
            transport1.AcceptAsync().Returns(x => UniTask.FromResult(conn1), x => UniTask.FromResult<IConnection>(null));
            // transport2 task never ends
            transport2.AcceptAsync().Returns(x => UniTask.FromResult(conn2), x => UniTask.FromResult<IConnection>(null));

            IConnection accepted1 = await transport.AcceptAsync();
            IConnection accepted2 = await transport.AcceptAsync();

            Assert.That(new[] { accepted1, accepted2 }, Is.EquivalentTo(new[] { conn1, conn2 }));

            Assert.That(await transport.AcceptAsync(), Is.Null);
        });

        [UnityTest]
        public IEnumerator Listen() => UniTask.ToCoroutine(async () =>
        {
            transport1.ListenAsync().Returns(UniTask.CompletedTask);
            transport2.ListenAsync().Returns(UniTask.CompletedTask);
            await transport.ListenAsync();

            _ = transport1.Received().ListenAsync();
            _ = transport2.Received().ListenAsync();

        });

        [Test]
        public void Disconnect()
        {
            transport.Disconnect();

            transport1.Received().Disconnect();
            transport2.Received().Disconnect();
        }

        [Test]
        public void ServerUri()
        {
            transport1.ServerUri().Returns(new[] { new Uri("tcp4://myserver") });

            Assert.That(transport.ServerUri(), Is.EquivalentTo(new[] { new Uri("tcp4://myserver") }));
        }

        [Test]
        public void Scheme1()
        {
            transport1.Scheme.Returns(new[] { "tcp4" });

            Assert.That(transport.Scheme, Is.EquivalentTo(new[] { "tcp4" }));
        }

        [Test]
        public void SchemeNone()
        {
            transport1.Scheme.Returns(new[] { "yomama" });
            transport2.Scheme.Returns(new[] { "pepe" });
            transport1.Supported.Returns(false);
            transport2.Supported.Returns(false);

            
            Assert.That(transport.Scheme, Is.Empty);
        }

        [UnityTest]
        public IEnumerator Connect() => UniTask.ToCoroutine(async () =>
        {
            transport1.Scheme.Returns(new[] { "yomama" });
            transport2.Scheme.Returns(new[] { "tcp4" });

            transport1.ConnectAsync(Arg.Any<Uri>())
                .Returns(UniTask.FromException<IConnection>(new ArgumentException("Invalid protocol")));

            // transport2 gives a connection
            transport2.ConnectAsync(Arg.Any<Uri>())
                .Returns(UniTask.FromResult(conn2));

            IConnection accepted1 = await transport.ConnectAsync(new Uri("tcp4://localhost"));

            Assert.That(accepted1, Is.SameAs(conn2));
        });

        [UnityTest]
        public IEnumerator CannotConnect() => UniTask.ToCoroutine(async () =>
        {
            transport1.ConnectAsync(Arg.Any<Uri>())
                .Returns(UniTask.FromException<IConnection>(new ArgumentException("Invalid protocol")));

            // transport2 gives a connection
            transport2.ConnectAsync(Arg.Any<Uri>())
                .Returns(UniTask.FromException<IConnection>(new ArgumentException("Invalid protocol")));

            try
            {
                _ = await transport.ConnectAsync(new Uri("tcp4://localhost"));
                Assert.Fail("Should not be able to connect if none of the transports can connect");
            }
            catch (ArgumentException)
            {
                // ok
            }
        });
    }
}