#region Copyright (c) 2003-2005, Luke T. Maxon

/********************************************************************************************************************
'
' Copyright (c) 2003-2005, Luke T. Maxon
' All rights reserved.
' 
' Redistribution and use in source and binary forms, with or without modification, are permitted provided
' that the following conditions are met:
' 
' * Redistributions of source code must retain the above copyright notice, this list of conditions and the
' 	following disclaimer.
' 
' * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
' 	the following disclaimer in the documentation and/or other materials provided with the distribution.
' 
' * Neither the name of the author nor the names of its contributors may be used to endorse or 
' 	promote products derived from this software without specific prior written permission.
' 
' THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
' WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
' PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
' ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
' LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
' INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
' OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
' IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
'
'*******************************************************************************************************************/

#endregion

using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Extensions.Forms;
using NUnit.Extensions.Forms.Util;

namespace AW.Test.Helpers
{
  /// <summary>
  ///   One of three base classes for your NUnitForms tests.  This one can be
  ///   used by people who do not need or want "built-in" Assert functionality.
  ///   This is the recommended base class for all unit tests that use NUnitForms.
  /// </summary>
  /// <remarks>
  ///   You should probably extend this class to create all of your test fixtures.  The benefit is that
  ///   this class implements setup and teardown methods that clean up after your test.  Any forms that
  ///   are created and displayed during your test are cleaned up by the tear down method.  This base
  ///   class also provides easy access to keyboard and mouse controllers.  It has a method that allows
  ///   you to set up a handler for modal dialog boxes.  It allows your tests to run on a separate
  ///   (usually hidden) desktop so that they are faster and do not interfere with your normal desktop
  ///   activity.  If you want custom setup and teardown behavior, you should override the virtual
  ///   Setup and TearDown methods.  Do not use the setup and teardown attributes in your child class.
  /// </remarks>
  public class NUnitFormMSTest
  {
    private static readonly FieldInfo IsUserInteractive =
      typeof (SystemInformation).GetField("isUserInteractive", BindingFlags.Static | BindingFlags.NonPublic);

    private ModalFormTester _modal;

    private Desktop _testDesktop;

    /// <summary>
    ///   True if the modal handlers for this test have been verified; else false.
    /// </summary>
    /// <remarks>
    ///   It would be better form to make this private and provide a protected getter property, though
    ///   that could break existing tests.
    /// </remarks>
    private bool _verified;

    /// <summary>
    ///   This property controls whether the separate hidden desktop is displayed for the duration of
    ///   this test.  You will need to override and return true from this property if your test makes
    ///   use of the keyboard or mouse controllers.  (The hidden desktop cannot accept user input.)  For
    ///   tests that do not use the keyboard and mouse controller (most should not) you don't need to do
    ///   anything with this.  The default behavior is fine.
    /// </summary>
    protected virtual bool DisplayHidden
    {
      get { return false; }
    }

    /// <summary>
    ///   This property controls whether a separate desktop is used at all. Tests on the separate desktop
    ///   are faster and safer (there is no danger of keyboard or mouse input going to your own separate
    ///   running applications).  However, it fails on some systems; also, it is not possible to unlock
    ///   by hand a blocked test (e.g. due to a modal form). In order to enable it, you can override
    ///   this method from your test class to return true. Or you can set an environment variable called
    ///   "UseHiddenDesktop" and set that to "true".
    /// </summary>
    protected virtual bool UseHidden
    {
      get
      {
        var useHiddenDesktop = Environment.GetEnvironmentVariable("UseHiddenDesktop");
        if (useHiddenDesktop != null && useHiddenDesktop.ToUpper().Equals("TRUE"))
        {
          return true;
        }
        return false;
      }
    }


    /// <summary>
    ///   Records a single shot modal form handler. The handler receives as arguments the title of the window,
    ///   its handle, and the corresponding form (null if it is not a form, i.e. a dialog box). The handler is
    ///   single shot: it is removed after being run; therefore, if it is expected to trigger a new modal form,
    ///   it should install a new handler before returning. The handler can work on dialog boxes by creating
    ///   a message box tester or file dialog tester, passing the handle of the box (its second argument) to the
    ///   tester's constructor. The tester constructors taking as argument the box title are unreliable and deprecated.
    /// </summary>
    protected ModalFormHandler ModalFormHandler
    {
      get { return _modal.FormHandler; }
      set { _modal.FormHandler = value; }
    }


    /// <summary>
    ///   This is the base classes setup method.  It will be called by NUnit before each test.
    ///   You should not have anything to do with it.
    /// </summary>
    public void Init()
    {
      _verified = false;


      if (!SystemInformation.UserInteractive)
      {
        IsUserInteractive.SetValue(null, true);
      }

      if (UseHidden)
      {
        _testDesktop = new Desktop("NUnitForms Test Desktop", DisplayHidden);
      }

      _modal = new ModalFormTester();
      GetMessageHook.InstallHook();
      Setup();
    }


    /// <summary>
    ///   Override this Setup method if you have custom behavior to execute before each test
    ///   in your fixture.
    /// </summary>
    protected virtual void Setup()
    {
    }

    /// <summary>
    ///   This method is called by NUnit after each test runs.  If you have custom
    ///   behavior to run after each test, then override the TearDown method and do
    ///   it there.  That method is called at the beginning of this one.
    ///   You should not need to do anything with it.  Do not call it.
    ///   If you do call it, call it as the last thing you do in your test.
    /// </summary>
    public void Verify()
    {
      try
      {
        TearDown();
        GetMessageHook.RemoveHook();

        if (ModalFormHandler == null)
        {
          // Make an effort to ensure that no window message is left dangling
          // Such a message might cause an unexpected dialog box
          for (var i = 0; i < 10; ++i)
          {
            Application.DoEvents();
          }
        }

        if (!_verified)
        {
          _verified = true;
          var allForms = new FormFinder().FindAll();

          foreach (var form in allForms)
          {
            if (!KeepAlive.ShouldKeepAlive(form))
            {
              form.Dispose();
              form.Hide();
            }
          }

          var modalResult = _modal.Verify();
          if (!modalResult.AllModalsShown)
          {
            throw new FormsTestAssertionException("Expected Modal Form did not show");
          }
          if (modalResult.UnexpectedModalWasShown)
          {
            var msg = modalResult.UnexpectedModals.Aggregate("Unexpected modals: ", (current, mod) => current + (mod + ", "));
            throw new FormsTestAssertionException(msg);
          }

          _modal.Dispose();

          if (UseHidden)
          {
            _testDesktop.Dispose();
          }
        }
      }
      finally
      {
        _modal.Dispose();
      }
    }

    /// <summary>
    ///   Override this TearDown method if you have custom behavior to execute after each test
    ///   in your fixture.
    /// </summary>
    public virtual void TearDown()
    {
    }

    /// <summary>
    ///   Deprecated in favor of ModalFormHandler/ModalDialogHandler.
    /// </summary>
    [Obsolete]
    private void ExpectModal(string name, ModalFormActivated handler, bool expected)
    {
      _modal.ExpectModal(name, handler, expected);
    }

    /// <summary>
    ///   Deprecated in favor of ModalFormHandler/ModalDialogHandler.
    /// </summary>
    [Obsolete]
    private void ExpectModal(string name, string handlerName, bool expected)
    {
      ExpectModal(name,
        (ModalFormActivated) Delegate.CreateDelegate(typeof (ModalFormActivated), this, handlerName),
        expected);
    }

    /// <summary>
    ///   Deprecated in favor of ModalFormHandler/ModalDialogHandler.
    /// </summary>
    [Obsolete]
    private void ExpectModal(string name, string handlerName)
    {
      ExpectModal(name, handlerName, true);
    }
  }
}