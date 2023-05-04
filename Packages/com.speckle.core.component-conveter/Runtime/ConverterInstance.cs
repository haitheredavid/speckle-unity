﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Speckle.ConnectorUnity.Models;
using Speckle.Core.Models;
using UnityEngine;

namespace Speckle.ConnectorUnity.Converter
{
    public sealed class ConverterInstance
    {

        private ScriptableConverter _converter;
        private CancellationToken _token;
        private Base _data;
        private SpeckleLayer _topLayer;


        public ConverterInstance(ScriptableConverter converter, Base @base)
        {
            _converter = converter;
            _data = @base;
        }

        public void Run()
        {
            // create an object that will be the top most parent 
            _topLayer = NewLayer("Parent");
            ConvertToScene(_data, _topLayer);
        }

        /// <summary>
        /// For handling creating objects in a scene without handling how the data needs to converted
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        void ConvertToScene(object obj, SpeckleLayer layer)
        {
            // 1: Object is supported, so lets convert the object and it's data 
            if (obj.IsBase(out var @base))
            {
                if (_converter.CanConvertToNative(@base))
                {
                    var go = _converter.CreateInstance(@base, layer.transform);
                    layer.Add(go);
                }

                // 2: Check for the properties of an object. There might be additional objects that we want to convert as well
                foreach (var kvp in @base.GetMembers(DynamicBaseMemberType.Dynamic))
                {
                    if (_token.IsCancellationRequested)
                    {
                        //TODO: throw error properly
                        // return;
                    }

                    if (kvp.Value == null) continue;

                    // 2b: Prop is a list!
                    if (kvp.Value.IsList())
                    {
                        var propList = ToList(kvp.Value);
                        var propLayer = NewLayer(kvp.Key);
                        layer.AddAndParent(propLayer);

                        foreach (var propItem in propList)
                        {
                            if (propItem == null) continue;

                            ConvertToScene(obj, propLayer);
                        }
                    }

                }
                return;
            }

            // 3: might be a list (not sure if this is necessary
            if (obj.IsList())
            {
                var list = ToList(obj);
                foreach (var item in list)
                {
                    ConvertToScene(item, layer);
                }
            }

        }

        static IEnumerable<object> ToList(object obj)
        {
            return((IEnumerable)obj).Cast<object>();
        }

        static SpeckleLayer NewLayer(string n)
        {
            return new GameObject(n).AddComponent<SpeckleLayer>();
        }


        static GameObject CheckConvertedFormat(object obj)
        {
            switch (obj)
            {
                case GameObject o:
                    return o;
                case MonoBehaviour o:
                    return o.gameObject;
                case Component o:
                    return o.gameObject;
                default:
                    SpeckleUnity.Console.Warn($"Object converted to unity from speckle is not supported {obj.GetType()}");
                    return null;
            }
        }

        // private GameObject HandleBase(Base data, SpeckleLayer parent)
        // {
        //     // 1: Object is supported, so lets convert the object and it's data 
        //     GameObject obj = _converter.CanConvertToNative(data) ? _converter.CreateInstance(data, parent.transform) : parent.gameObject;
        //
        //     // 2: Check for the properties of an object. There might be additional objects that we want to convert as well
        //     foreach (var kvp in data.GetMembers(DynamicBaseMemberType.Dynamic))
        //     {
        //         if (_token.IsCancellationRequested)
        //         {
        //             //TODO: throw error properly
        //             // return;
        //         }
        //
        //         if (kvp.Value == null) continue;
        //
        //         // 2a: Member is a speckle object and we want to convert that bad pup to a game object
        //         if (kvp.Value.IsBase(out var child))
        //         {
        //             HandleBase(child, obj.transform);
        //             continue;
        //         }
        //
        //         // 2b: A prop is a list!
        //         if (kvp.Value.IsList())
        //         {
        //             var propList = ToList(kvp.Value);
        //             var propLayer = NewLayer(kvp.Key);
        //
        //             foreach (var propItem in propList)
        //             {
        //                 if (propItem == null) continue;
        //
        //                 if (propItem.IsBase(out var propChild))
        //                 {
        //                     parentLayer.Add(HandleBase(propChild, parentLayer.transform));
        //                 }
        //                 else if (propItem.IsList())
        //                 {
        //                     parentLayer.Add(ListToLayer(NewLayer("child"), ToList(propItem)));
        //                 }
        //
        //             }
        //             ListToLayer(NewLayer(kvp.Key), ToList(kvp.Value));
        //             // add to hierarchy 
        //             // hierarchy.Add(layer);
        //             continue;
        //         }
        //
        //
        //         Debug.LogWarning("Unhandled");
        //     }
        //
        //
        // }



        // SpeckleLayer ListToLayer(SpeckleLayer parentLayer, IEnumerable<object> data)
        // {
        //     foreach (var item in data)
        //     {
        //         if (_token.IsCancellationRequested)
        //         {
        //             return null;
        //         }
        //
        //         if (item == null) continue;
        //
        //         if (item.IsBase(out var child))
        //         {
        //             parentLayer.Add(HandleBase(child, parentLayer.transform));
        //         }
        //         else if (item.IsList())
        //         {
        //             parentLayer.Add(ListToLayer(NewLayer("child"), ToList(item)));
        //         }
        //     }
        //     // parentLayer.ParentObjects(parentLayer.transform);
        //     return parentLayer;
        // }




    }

}
