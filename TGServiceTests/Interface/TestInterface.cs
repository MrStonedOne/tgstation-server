﻿using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TGServiceInterface.Tests
{
	/// <summary>
	/// Tests for <see cref="Interface"/>
	/// </summary>
	[TestClass]
	public class TestInterface
	{
		/// <summary>
		/// Test that <see cref="Interface.SetBadCertificateHandler(Func{string, bool})"/> can execute successfully
		/// </summary>
		[TestMethod]
		public void TestSetBadCertificateHandler()
		{
			Func<string, bool> func = (message) =>
			{
				Assert.IsFalse(String.IsNullOrWhiteSpace(message));
				return true;
			};
			Interface.SetBadCertificateHandler(func);
		}

		/// <summary>
		/// Test that <see cref="Interface.SetBadCertificateHandler(Func{string, bool})"/> properly sets <see cref="ServicePointManager.ServerCertificateValidationCallback"/>
		/// </summary>
		[TestMethod]
		public void TestBadCertificateHandler()
		{
			var ran = false;
			Interface.SetBadCertificateHandler(_ =>
			{
				ran = true;
				return true;
			});
			ServicePointManager.ServerCertificateValidationCallback(this, new System.Security.Cryptography.X509Certificates.X509Certificate(), new System.Security.Cryptography.X509Certificates.X509Chain(), System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors);
			Assert.IsTrue(ran);
		}

		/// <summary>
		/// Creates a remote configured <see cref="Interface"/> pointing at an invalid address
		/// </summary>
		/// <returns>The created <see cref="Interface"/></returns>
		Interface CreateFakeRemoteInterface()
		{
			return new Interface("some.fake.url.420", 34752, "user", "password");
		}

		/// <summary>
		/// Test that <see cref="Interface.Interface"/> can execute successfully and creates a local connection
		/// </summary>
		[TestMethod]
		public void TestLocalInstantiation()
		{
			Assert.IsFalse(new Interface().IsRemoteConnection);
		}

		/// <summary>
		/// Test that <see cref="Interface(string, ushort, string, string)"/> can execute successfully
		/// </summary>
		[TestMethod]
		public void TestRemoteInstatiation()
		{
			Assert.IsTrue(CreateFakeRemoteInterface().IsRemoteConnection);
		}

		[TestMethod]
		public void TestCopyRemoteInterface()
		{
			var first = CreateFakeRemoteInterface();
			var second = new Interface(first);
			Assert.AreEqual(first.HTTPSURL, second.HTTPSURL);
			Assert.AreEqual(first.HTTPSPort, second.HTTPSPort);
			Assert.IsTrue(second.IsRemoteConnection);
		}
	}
}
