﻿using lib12.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;

namespace nex.Utilities
{
        /// <summary>
        /// Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
        /// and executes callback methods (i.e. handlers) registered for properties of that object.
        /// </summary>
        /// <typeparam name="TPropertySource">The type of object to monitor for property changes.</typeparam>
        public class PropertyObserver<TPropertySource> : IWeakEventListener
            where TPropertySource : INotifyPropertyChanged
        {
            #region Constructor

            /// <summary>
            /// Initializes a new instance of PropertyObserver, which
            /// observes the 'propertySource' object for property changes.
            /// </summary>
            /// <param name="propertySource">The object to monitor for property changes.</param>
            public PropertyObserver(TPropertySource propertySource)
            {
                if (propertySource == null)
                    throw new ArgumentNullException("propertySource");

                _propertySourceRef = new WeakReference(propertySource);
                _propertyNameToHandlerMap = new Dictionary<string, Action<TPropertySource>>();
                propertyNameToActionMap = new Dictionary<string, Action>();
            }

            #endregion // Constructor

            #region Public Methods

            #region RegisterHandler

            /// <summary>
            /// Registers a callback to be invoked when the PropertyChanged event has been raised for the specified property.
            /// </summary>
            /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
            /// <param name="handler">The callback to invoke when the property has changed.</param>
            /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
            public PropertyObserver<TPropertySource> RegisterHandler(
                Expression<Func<TPropertySource, object>> expression,
                Action<TPropertySource> handler)
            {
                if (expression == null)
                    throw new ArgumentNullException("expression");

                string propertyName = this.GetPropertyName(expression);
                if (String.IsNullOrEmpty(propertyName))
                    throw new ArgumentException("'expression' did not provide a property name.");

                if (handler == null)
                    throw new ArgumentNullException("handler");

                TPropertySource propertySource = this.GetPropertySource();
                if (propertySource != null)
                {
                    _propertyNameToHandlerMap[propertyName] = handler;
                    PropertyChangedEventManager.AddListener(propertySource, this, propertyName);
                }

                return this;
            }

            public PropertyObserver<TPropertySource> RegisterHandler(Expression<Func<TPropertySource, object>> expression, Action handler)
            {
                if (expression == null)
                    throw new ArgumentNullException("expression");

                string propertyName = this.GetPropertyName(expression);
                if (String.IsNullOrEmpty(propertyName))
                    throw new ArgumentException("'expression' did not provide a property name.");

                if (handler == null)
                    throw new ArgumentNullException("handler");

                TPropertySource propertySource = this.GetPropertySource();
                if (propertySource != null)
                {
                    propertyNameToActionMap[propertyName] = handler;
                    PropertyChangedEventManager.AddListener(propertySource, this, propertyName);
                }

                return this;
            }

            #endregion // RegisterHandler

            #region UnregisterHandler

            /// <summary>
            /// Removes the callback associated with the specified property.
            /// </summary>
            /// <param name="propertyName">A lambda expression like 'n => n.PropertyName'.</param>
            /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
            public PropertyObserver<TPropertySource> UnregisterHandler(Expression<Func<TPropertySource, object>> expression)
            {
                if (expression == null)
                    throw new ArgumentNullException("expression");

                string propertyName = this.GetPropertyName(expression);
                if (String.IsNullOrEmpty(propertyName))
                    throw new ArgumentException("'expression' did not provide a property name.");

                TPropertySource propertySource = this.GetPropertySource();
                if (propertySource != null)
                {
                    if (_propertyNameToHandlerMap.ContainsKey(propertyName))
                    {
                        _propertyNameToHandlerMap.Remove(propertyName);
                        PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
                    }
                    else if (propertyNameToActionMap.ContainsKey(propertyName))
                    {
                        propertyNameToActionMap.Remove(propertyName);
                        PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
                    }
                }

                return this;
            }

            #endregion // UnregisterHandler

            #endregion // Public Methods

            #region Private Helpers

            #region GetPropertyName

            string GetPropertyName(Expression<Func<TPropertySource, object>> expression)
            {
                var lambda = expression as LambdaExpression;
                MemberExpression memberExpression;
                if (lambda.Body is UnaryExpression)
                {
                    var unaryExpression = lambda.Body as UnaryExpression;
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                else
                {
                    memberExpression = lambda.Body as MemberExpression;
                }

                Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.PropertyName'");

                if (memberExpression != null)
                {
                    var propertyInfo = memberExpression.Member as PropertyInfo;

                    return propertyInfo.Name;
                }

                return null;
            }

            #endregion // GetPropertyName

            #region GetPropertySource

            TPropertySource GetPropertySource()
            {
                try
                {
                    return (TPropertySource)_propertySourceRef.Target;
                }
                catch
                {
                    return default(TPropertySource);
                }
            }

            #endregion // GetPropertySource

            #endregion // Private Helpers

            #region Fields

            readonly Dictionary<string, Action<TPropertySource>> _propertyNameToHandlerMap;
            readonly Dictionary<string, Action> propertyNameToActionMap;
            readonly WeakReference _propertySourceRef;

            #endregion // Fields

            #region IWeakEventListener Members

            bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
            {
                if (managerType == typeof(PropertyChangedEventManager))
                {
                    string propertyName = ((PropertyChangedEventArgs)e).PropertyName;
                    TPropertySource propertySource = (TPropertySource)sender;

                    if (String.IsNullOrEmpty(propertyName))
                    {
                        // When the property name is empty, all properties are considered to be invalidated.
                        // Iterate over a copy of the list of handlers, in case a handler is registered by a callback.
                        foreach (Action<TPropertySource> handler in _propertyNameToHandlerMap.Values.ToArray())
                            handler(propertySource);

                        foreach (var handler in propertyNameToActionMap.Values.ToArray())
                            handler();

                        return true;
                    }
                    else
                    {
                        Action<TPropertySource> handler;
                        if (_propertyNameToHandlerMap.TryGetValue(propertyName, out handler))
                        {
                            handler(propertySource);
                            return true;
                        }
                        else
                        {
                            var action = propertyNameToActionMap.GetValueOrDefault(propertyName);
                            if (action != null)
                            {
                                action();
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            #endregion
        }
}
