using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace T2TFramework.Data
{
    
    [Serializable]
    public struct SaveDataContainer
    {
        public Dictionary<string, int> IntMap;
        public Dictionary<string, float> FloatMap;
        public Dictionary<string, bool> BoolMap;
        public Dictionary<string, string> StringMap;
        public Dictionary<string, IData> DataMap;
        public Dictionary<string, Color> ColorMap;
        public Dictionary<string, Vector2> Vector2Map;
        public Dictionary<string, Vector3> Vector3Map;
        public Dictionary<string, Vector4> Vector4Map;
    }
    [Serializable]
    public struct TransformContainer
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public bool TryLoad(Transform transform)
        {
            if (transform is null)
                return false;
            transform.position = Position;
            transform.rotation = Quaternion.Euler(Rotation);
            transform.localScale = Scale;
            return true;
        }
        public static implicit  operator TransformContainer(Transform transform)
        {
            return new TransformContainer()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles,
                Scale = transform.localScale,
            };
        }
    }
    [Serializable]
    public struct RectTransformContainer
    {
        public Vector2 Pivot;
        public Vector2 AnchoredPosition;
        public Vector2 AnchorMax;
        public Vector2 AnchorMin;
        public Vector2 OffsetMax;
        public Vector2 OffsetMin;
        public Vector2 SizeDelta;

        public bool TryLoad(ref RectTransform rectTransform)
        {
            if (rectTransform is null)
                return false;
            rectTransform.pivot = Pivot;
            rectTransform.anchoredPosition = AnchoredPosition;
            rectTransform.offsetMax = OffsetMax;
            rectTransform.sizeDelta = SizeDelta;
            rectTransform.anchorMax = AnchorMax;
            rectTransform.anchorMin = AnchorMin;
            rectTransform.offsetMin = OffsetMin;
            return true;
        }
        public static implicit operator RectTransformContainer(RectTransform rectTransform)
        {
            RectTransformContainer container = new()
            {
                Pivot = rectTransform.pivot,
                AnchoredPosition = rectTransform.anchoredPosition,
                AnchorMax = rectTransform.anchorMax,
                AnchorMin = rectTransform.anchorMin,
                OffsetMax = rectTransform.offsetMax,
                OffsetMin = rectTransform.offsetMin,
                SizeDelta = rectTransform.sizeDelta
            };
            return container;
        }
    }
    [Serializable]
    public struct ColorContainer
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ColorContainer(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static implicit operator ColorContainer(Color color)
        {
            return new ColorContainer(color.r, color.g, color.b, color.a);
        }

        public static implicit operator Color(ColorContainer container)
        {
            return new Color(container.r, container.g, container.b, container.a);
        }

    }
    
    public interface IData{}

    public static partial class EasyData
    {
        private static Dictionary<string, IData> _dataMap = new();
        public static void SetData<T>(string key,T data) where T : struct,IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            Delete(key);
            _dataMap[key] = data;
        }

        public static T GetData<T>(string key,T defaultData = default)  where T : struct,IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            if (!_dataMap.ContainsKey(key))
                return defaultData;
            else
                return _dataMap[key] is T ? (T)_dataMap[key] : default;
        }

        public static bool TryGetData<T>(string key,out T value) where T : struct,IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            value = default;
            if (!_dataMap.ContainsKey(key))
                return false;
            value = _dataMap[key] is T ? (T)_dataMap[key] : default;
            return true;
        }
    }
    public static partial class EasyData
    {
        private static Dictionary<string, int> _intMap = new();
        private static Dictionary<string, float> _floatMap = new();
        private static Dictionary<string, bool> _boolMap = new();
        private static Dictionary<string, string> _stringMap = new();
        //Unity数据
        private static Dictionary<string, Color> _colorMap = new();
        private static Dictionary<string, Vector2> _vector2Map = new();
        private static Dictionary<string, Vector3> _vector3Map = new();
        private static Dictionary<string, Vector4> _vector4Map = new();
        //未投放使用
        private static Dictionary<string, TransformContainer> _transformMap = new();
        private static Dictionary<string, RectTransformContainer> _rectTransformMap = new();
        
        
        private static string _defaultDataPath = $"{Application.dataPath}/{_fileName}";
        private static string _fileName = Guid.NewGuid().ToString()+".data";
        public static event Action<DataSaveState> OnDataSave;
        public static event Action<DataSaveState> OnDataLoad;
        public static event Action<Exception> OnThrowException;
        
    }
    //设置数据
    public static partial class EasyData
    {
        public static void SetFloat(string key, float value)
        {
            Delete(key);
            _floatMap.Add(key,value);
        }
        public static void SetBool(string key, bool value)
        {
            Delete(key);
            _boolMap.Add(key,value);
        }
        public static void SetString(string key, string value)
        {
            Delete(key);
            _stringMap.Add(key,value);
        }
        public static void SetInt(string key, int value)
        {
            Delete(key);
            _intMap.Add(key,value);
        }

        public static void SetColor(string key, Color value)
        {
            Delete(key);
            _colorMap.Add(key,value);
        }
        
        public static void SetVector2(string key, Vector2 value)
        {
            Delete(key);
            _vector2Map.Add(key,value);
        }
        public static void SetVector3(string key, Vector3 value)
        {
            Delete(key);
            _vector3Map.Add(key,value);
        }
        public static void SetVector4(string key, Vector4 value)
        {
            Delete(key);
            _vector4Map.Add(key,value);
        }
     
    }
    //尝试获取数据,如果数据不存在，则会使用默认数据，
    public static partial class EasyData
    {
        public static bool TryGetFloat(string key,out float value,float defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_floatMap.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _floatMap.Add(key,value);
                return false;
            }
            value = _floatMap[key];
            return true;
        }
        public static bool TryGetBool(string key,out bool value,bool defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_boolMap.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _boolMap.Add(key,value);
                return false;
            }
            value = _boolMap[key];
            return true;
        }
        public static bool TryGetString(string key,out string value , string defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_stringMap.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _stringMap.Add(key,value);
                return false;
            }
            value = _stringMap[key];
            return true;
        }
        public static bool TryGetInt(string key,out int value,int defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_intMap.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _intMap.Add(key,value);
                return false;
            }
            value = _intMap[key];
            return true;
        }

        public static bool TryGetColor(string key, out Color value , Color defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_colorMap.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _colorMap.Add(key,value);
                return false;
            }
            value = _colorMap[key];
            return true;
        }
        public static bool TryGetVector2(string key, out Vector2 value , Vector2 defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_vector2Map.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _vector2Map.Add(key,value);
                return false;
            }
            value = _vector2Map[key];
            return true;
        }
        public static bool TryGetVector3(string key, out Vector3 value , Vector3 defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_vector3Map.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _vector3Map.Add(key,value);
                return false;
            }
            value = _vector3Map[key];
            return true;
        }
        public static bool TryGetVector4(string key , out Vector4 value , Vector4 defaultValue = default,bool saveDefaultValue = false)
        {
            value = defaultValue;
            if (!_vector4Map.ContainsKey(key))
            {
                if(saveDefaultValue)
                    _vector4Map.Add(key,value);
                return false;
            }
            value = _vector4Map[key];
            return true;
        }
    }
   

    //数据全局操作
    public static partial class EasyData
    {
        private static SaveDataContainer GetContainer()
        {
            SaveDataContainer container = new()
            {
                IntMap = _intMap,
                FloatMap = _floatMap,
                StringMap = _stringMap,
                BoolMap = _boolMap,
                DataMap = _dataMap,
                ColorMap = _colorMap,
                Vector2Map = _vector2Map,
                Vector3Map = _vector3Map,
                Vector4Map = _vector4Map,
            };
            return container;
        }

        private static void LoadContainer(SaveDataContainer container)
        {
            _floatMap = container.FloatMap;
            _boolMap = container.BoolMap;
            _stringMap = container.StringMap;
            _intMap = container.IntMap;
            _dataMap = container.DataMap;
            _colorMap = container.ColorMap;
            _vector2Map = container.Vector2Map;
            _vector3Map = container.Vector3Map;
            _vector4Map = container.Vector4Map;
        }
        public static bool HasKey(string key)
        {
            if (_intMap.ContainsKey(key))
                return true;
            if (_floatMap.ContainsKey(key))
                return true;
            if (_boolMap.ContainsKey(key))
                return true;
            if (_stringMap.ContainsKey(key))
                return true;
            if (_colorMap.ContainsKey(key))
                return true;
            if (_vector2Map.ContainsKey(key))
                return true;
            if (_vector3Map.ContainsKey(key))
                return true;
            if (_vector4Map.ContainsKey(key))
                return true;
            return false;
        }

        public static bool HasKey(string key, EasyValueType valueType)
        {
            return valueType switch
            {
                EasyValueType.Boolean =>  _boolMap.ContainsKey(key),
                EasyValueType.Int =>  _intMap.ContainsKey(key),
                EasyValueType.String =>  _stringMap.ContainsKey(key),
                EasyValueType.Float =>  _floatMap.ContainsKey(key),
                EasyValueType.Color =>  _colorMap.ContainsKey(key),
                EasyValueType.Vector2 =>  _vector2Map.ContainsKey(key),
                EasyValueType.Vector3 =>  _vector3Map.ContainsKey(key),
                EasyValueType.Vector4 =>  _vector4Map.ContainsKey(key),
                _ => _dataMap.ContainsKey(key)
            };
        }

        public static void Delete(string key)
        {
            _intMap.Remove(key);
            _floatMap.Remove(key);
            _boolMap.Remove(key);
            _stringMap.Remove(key);
            _dataMap.Remove(key);
            _colorMap.Remove(key);
            _vector2Map.Remove(key);
            _vector3Map.Remove(key);
            _vector4Map.Remove(key);
        }

        public static void Delete(string key, EasyValueType valueType)
        {
            switch (valueType)
            {
                case EasyValueType.Boolean:
                    _boolMap.Remove(key);
                    break;
                case EasyValueType.Float:
                    _floatMap.Remove(key);
                    break;
                case EasyValueType.String:
                    _stringMap.Remove(key);
                    break;
                case EasyValueType.Int:
                    _intMap.Remove(key);
                    break;
                case EasyValueType.Color:
                    _colorMap.Remove(key);
                    break;
                case EasyValueType.Vector2:
                    _vector2Map.Remove(key);
                    break;
                case EasyValueType.Vector3:
                    _vector3Map.Remove(key);
                    break;
                case EasyValueType.Vector4:
                    _vector4Map.Remove(key);
                    break;
                default:
                    _dataMap.Remove(key);
                    break;
            }
        }

        public static void DeleteAll()
        {
            _intMap.Clear();
            _floatMap.Clear();
            _boolMap.Clear();
            _stringMap.Clear();
            _dataMap.Clear();
            _colorMap.Clear();
            _vector2Map.Clear();
            _vector3Map.Clear();
            _vector4Map.Clear();
        }

        public static EasyValueType GetValueType(string key)
        {
            if (_intMap.ContainsKey(key))
                return EasyValueType.Int;
            if (_boolMap.ContainsKey(key))
                return EasyValueType.Boolean;
            if (_floatMap.ContainsKey(key))
                return EasyValueType.Float;
            if (_stringMap.ContainsKey(key))
                return EasyValueType.String;
            if (_colorMap.ContainsKey(key))
                return EasyValueType.Color;
            if (_vector2Map.ContainsKey(key))
                return EasyValueType.Vector2;
            if (_vector3Map.ContainsKey(key))
                return EasyValueType.Vector3;
            if (_vector4Map.ContainsKey(key))
                return EasyValueType.Vector4;
            return EasyValueType.None;
        }
        
    }
    //持久化数据操作
    public static partial class EasyData
    {
        /// <summary>
        /// 设置数据保存位置
        /// </summary>
        /// <param name="path"></param>
        public static void SetDataPath(string path)
        {
            _defaultDataPath = path;
        }

        public static string GetDataPath()
        {
            return _defaultDataPath;
        }

        public static void Save(string path)
        {
            Save(default,path);
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param>
        ///     <name>path</name>
        /// </param>
        /// <param name="callBack"></param>
        /// <param name="path"></param>
        public static async void Save(Action<DataSaveState,Exception> callBack = default,string path = default)
        {
            if (path == default)
                path = _defaultDataPath;
            else
                _defaultDataPath = path;
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                settings.NullValueHandling = NullValueHandling.Ignore;
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.Formatting = Formatting.Indented;
                settings.MissingMemberHandling = MissingMemberHandling.Ignore;
                settings.MaxDepth = 1;
                settings.ContractResolver = new ShouldSerialize();
                string jsonValue = JsonConvert.SerializeObject(GetContainer(),settings);
                Debug.Log($"数据大小：{System.Text.Encoding.UTF8.GetByteCount(jsonValue)}");
                Debug.Log($"保存路径: {path}");
                await File.WriteAllTextAsync(path, jsonValue, Encoding.UTF8);
            }
            catch (Exception e)
            {
                OnDataSave?.Invoke(DataSaveState.Error);
                OnThrowException?.Invoke(e);
                Debug.LogError(e);
                callBack?.Invoke(DataSaveState.Error,e);
            }
            OnDataSave?.Invoke(DataSaveState.Success);
        }

        public static void Load(string path)
        {
            Load(default, path);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="path"></param>
        public static  async void Load(Action<DataSaveState,Exception> callBack = default,string path = default)
        {
            if (path == default)
                path = _defaultDataPath;
            else
                _defaultDataPath = path;
            try
            {
                string jsonValue = await File.ReadAllTextAsync(path, Encoding.UTF8);
                Debug.Log($"数据大小：{System.Text.Encoding.UTF8.GetByteCount(jsonValue)}");
                Debug.Log($"读取路径: {path}");
                SaveDataContainer container = JsonConvert.DeserializeObject<SaveDataContainer>(jsonValue);
                LoadContainer(container);
            }
            catch (Exception e)
            {
                OnDataLoad?.Invoke(DataSaveState.Error);
                OnThrowException?.Invoke(e);
                Debug.LogError(e);
                callBack?.Invoke(DataSaveState.Error,e);
            }
            OnDataLoad?.Invoke(DataSaveState.Success);
        }
#if  UNITY_EDITOR
        public static bool TryGetKeys(EasyValueType valueType,out string[] keyValues)
        {
            keyValues = valueType switch
            {
                EasyValueType.Boolean => _boolMap.Keys.ToArray(),
                EasyValueType.Float => _floatMap.Keys.ToArray(),
                EasyValueType.String => _stringMap.Keys.ToArray(),
                EasyValueType.Int => _intMap.Keys.ToArray(),
                EasyValueType.Color => _colorMap.Keys.ToArray(),
                EasyValueType.Vector2 => _vector2Map.Keys.ToArray(),
                EasyValueType.Vector3 => _vector3Map.Keys.ToArray(),
                EasyValueType.Vector4 => _vector4Map.Keys.ToArray(),
                _ => null
            };
            return keyValues is null;
        }
        public static bool TrySearchKey(string keyValue, EasyValueType valueType, out string[] keyValues)
        {
            keyValues = null;
            if (!HasKey(keyValue, valueType))
                return false;
            List<string> list = UnityEngine.Pool.ListPool<string>.Get();
            string[] strings = valueType switch
            {
                EasyValueType.Boolean => _boolMap.Keys.ToArray(),
                EasyValueType.Float => _floatMap.Keys.ToArray(),
                EasyValueType.String => _stringMap.Keys.ToArray(),
                EasyValueType.Int => _intMap.Keys.ToArray(),
                EasyValueType.Color => _colorMap.Keys.ToArray(),
                EasyValueType.Vector2 => _vector2Map.Keys.ToArray(),
                EasyValueType.Vector3 => _vector3Map.Keys.ToArray(),
                EasyValueType.Vector4 => _vector4Map.Keys.ToArray(),
                _ => null
            };
            if (strings is null)
            {
                UnityEngine.Pool.ListPool<string>.Release(list);
                return false;
            }
            foreach (var str in strings)
            {
                if(str.Contains(keyValue))
                    list.Add(str);
            }
            keyValues = list.ToArray();
            UnityEngine.Pool.ListPool<string>.Release(list);
            return true;
        }
        public static string[] SearchKey(string keyValue, EasyValueType valueType)
        {
            List<string> list = UnityEngine.Pool.ListPool<string>.Get();
            string[] strings = valueType switch
            {
                EasyValueType.Boolean => _boolMap.Keys.ToArray(),
                EasyValueType.Float => _floatMap.Keys.ToArray(),
                EasyValueType.String => _stringMap.Keys.ToArray(),
                EasyValueType.Int => _intMap.Keys.ToArray(),
                EasyValueType.Color => _colorMap.Keys.ToArray(),
                EasyValueType.Vector2 => _vector2Map.Keys.ToArray(),
                EasyValueType.Vector3 => _vector3Map.Keys.ToArray(),
                EasyValueType.Vector4 => _vector4Map.Keys.ToArray(),
                _ => null
            };
            if (strings is null)
            {
                UnityEngine.Pool.ListPool<string>.Release(list);
                return null;
            }
            foreach (var str in strings)
            {
                if(str.Contains(keyValue))
                    list.Add(str);
            }
            var res = list.ToArray();
            UnityEngine.Pool.ListPool<string>.Release(list);
            return res;
        }

        public static (string, EasyValueType)[] GetAllKey()
        {
            List<(string, EasyValueType)> list = UnityEngine.Pool.ListPool<(string, EasyValueType)>.Get();
            foreach (var key in _intMap.Keys)  
            {
                    list.Add((key,EasyValueType.Int));
            }
            foreach (var key in _floatMap.Keys)  
            {
                    list.Add((key,EasyValueType.Float));
            }
            foreach (var key in _stringMap.Keys)  
            {
                    list.Add((key,EasyValueType.String));
            }
            foreach (var key in _boolMap.Keys)  
            {
                    list.Add((key,EasyValueType.Boolean));
            }
            foreach (var key in _colorMap.Keys)  
            {
                    list.Add((key,EasyValueType.Color));
            }
            foreach (var key in _vector2Map.Keys)  
            {
                    list.Add((key,EasyValueType.Vector2));
            }
            foreach (var key in _vector3Map.Keys)  
            {
                    list.Add((key,EasyValueType.Vector3));
            }
            foreach (var key in _vector4Map.Keys)  
            {
                    list.Add((key,EasyValueType.Vector4));
            }

            (string, EasyValueType)[] values = list.ToArray();
            UnityEngine.Pool.ListPool<(string, EasyValueType)>.Release(list);
            return values;
        }
        public static (string, EasyValueType)[] SearchKey(string keyValue,StringComparison comparison = StringComparison.Ordinal)
        {
            List<(string, EasyValueType)> list = UnityEngine.Pool.ListPool<(string, EasyValueType)>.Get();
            
            foreach (var key in _intMap.Keys)  
            {
                if("Int".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.Int));
            }
            foreach (var key in _floatMap.Keys)  
            {
                if("Float".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.Float));
            }
            foreach (var key in _stringMap.Keys)  
            {
                if("String".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.String));
            }
            foreach (var key in _boolMap.Keys)  
            {
                if("Bool".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.Boolean));
            }
            foreach (var key in _colorMap.Keys)  
            {
                if("Color".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.Color));
            }
            foreach (var key in _vector2Map.Keys)  
            {
                if("Vector2".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.Vector2));
            }
            foreach (var key in _vector3Map.Keys)  
            {
                if("Vector3".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.Vector3));
            }
            foreach (var key in _vector4Map.Keys)  
            {
                if("Vector4".Contains(keyValue,comparison)||key.Contains(keyValue,comparison))
                    list.Add((key,EasyValueType.Vector4));
            }

            (string, EasyValueType)[] values = list.ToArray();
            UnityEngine.Pool.ListPool<(string, EasyValueType)>.Release(list);
            return values;
        }
#endif
        
    }
    public static partial class EasyData
    {
        /// <summary>
        /// 同步数据到PlayerPrefs
        /// </summary>
        public static void SyncToPlayerPrefs()
        {
            string[] keys = _intMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetInt(key,_intMap[key]);
            }
            keys = _boolMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetInt(key,_boolMap[key]?0:1);
            }
            keys = _floatMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetFloat(key,_floatMap[key]);
            }
            keys = _stringMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetString(key,_stringMap[key]);
            }
        }
    }
    public enum DataSaveState
    {
        Success,Error
    }
    public enum EasyValueType
    {
        None,
        Float,
        Int,
        Boolean,
        String,
        Color,
        Vector2,
        Vector3,
        Vector4
    }

    public class ShouldSerialize : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property =  base.CreateProperty(member, memberSerialization);
            bool isPublic = member switch
            {
                FieldInfo f => f.IsPublic,
                PropertyInfo p => (!(p.SetMethod is  null || !p.SetMethod.IsPublic) && p.GetGetMethod().IsPublic),
                _ => false
            };
            property.ShouldSerialize = _ => isPublic;
            return property;
        }
    }
}