using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine.Pool;

namespace T2TFramework.Data
{
    /// <summary>
    /// 存档序列化结构体
    /// </summary>
    [Serializable]
    public class EasyDataContainer
    {
        /// <summary>
        /// EasyData对象池，每次取出时需要做初始化操作才可使用
        /// </summary>
        public static ObjectPool<EasyDataContainer> ContainerPool =
            new ObjectPool<EasyDataContainer>(() => new EasyDataContainer(), (container) => { },
                (container) => { container.Release(); });

        public string FilePath;
        public Dictionary<string, int> IntMap = DictionaryPool<string, int>.Get();
        public Dictionary<string, float> FloatMap = DictionaryPool<string, float>.Get();
        public Dictionary<string, bool> BoolMap = DictionaryPool<string, bool>.Get();
        public Dictionary<string, string> StringMap = DictionaryPool<string, string>.Get();
        public Dictionary<string, IData> DataMap = DictionaryPool<string, IData>.Get();
        public Dictionary<string, Color> ColorMap = DictionaryPool<string, Color>.Get();
        public Dictionary<string, Vector2> Vector2Map = DictionaryPool<string, Vector2>.Get();
        public Dictionary<string, Vector3> Vector3Map = DictionaryPool<string, Vector3>.Get();
        public Dictionary<string, Vector4> Vector4Map = DictionaryPool<string, Vector4>.Get();
        public Dictionary<string, EasyValueType> KeyToValueMap = DictionaryPool<string, EasyValueType>.Get();

        public EasyDataContainer()
        {
        }

        public EasyDataContainer(string filePath)
        {
            SetFilePath(filePath);
        }

        public EasyDataContainer Init()
        {
            IntMap = DictionaryPool<string, int>.Get();
            FloatMap = DictionaryPool<string, float>.Get();
            BoolMap = DictionaryPool<string, bool>.Get();
            StringMap = DictionaryPool<string, string>.Get();
            DataMap = DictionaryPool<string, IData>.Get();
            ColorMap = DictionaryPool<string, Color>.Get();
            Vector2Map = DictionaryPool<string, Vector2>.Get();
            Vector3Map = DictionaryPool<string, Vector3>.Get();
            Vector4Map = DictionaryPool<string, Vector4>.Get();
            KeyToValueMap = DictionaryPool<string, EasyValueType>.Get();
            FilePath = string.Empty;
            return this;
        }

        public EasyDataContainer Release()
        {
            DictionaryPool<string, float>.Release(FloatMap);
            DictionaryPool<string, bool>.Release(BoolMap);
            DictionaryPool<string, string>.Release(StringMap);
            DictionaryPool<string, int>.Release(IntMap);
            DictionaryPool<string, IData>.Release(DataMap);
            DictionaryPool<string, Color>.Release(ColorMap);
            DictionaryPool<string, Vector2>.Release(Vector2Map);
            DictionaryPool<string, Vector3>.Release(Vector3Map);
            DictionaryPool<string, Vector4>.Release(Vector4Map);
            DictionaryPool<string, EasyValueType>.Release(KeyToValueMap);
            FilePath = string.Empty;
            return this;
        }


        #region 数据操作1

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据</param>
        public void SetValue(string key, float value)
        {
            Delete(key);
            FloatMap.Add(key, value);
        }

        public void SetValue(string key, bool value)
        {
            Delete(key);
            BoolMap.Add(key, value);
        }

        public void SetValue(string key, string value)
        {
            Delete(key);
            StringMap.Add(key, value);
        }

        public void SetValue(string key, int value)
        {
            Delete(key);
            IntMap.Add(key, value);
        }

        public void SetValue(string key, Color value)
        {
            Delete(key);
            ColorMap.Add(key, value);
        }

        public void SetValue(string key, Vector2 value)
        {
            Delete(key);
            Vector2Map.Add(key, value);
        }

        public void SetValue(string key, Vector3 value)
        {
            Delete(key);
            Vector3Map.Add(key, value);
        }

        public void SetValue(string key, Vector4 value)
        {
            Delete(key);
            Vector4Map.Add(key, value);
        }

        /// <summary>
        /// 尝试获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">读取数据</param>
        /// <param name="defaultValue">默认数据</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out float value, float defaultValue = default)
        {
            value = defaultValue;
            if (!FloatMap.ContainsKey(key))
                return false;
            value = FloatMap[key];
            return true;
        }

        public bool TryGetValue(string key, out bool value, bool defaultValue = default)
        {
            value = defaultValue;
            if (!BoolMap.ContainsKey(key))
                return false;
            value = BoolMap[key];
            return true;
        }

        public bool TryGetValue(string key, out string value, string defaultValue = default)
        {
            value = defaultValue;
            if (!StringMap.ContainsKey(key))
                return false;
            value = StringMap[key];
            return true;
        }

        public bool TryGetValue(string key, out int value, int defaultValue = default)
        {
            value = defaultValue;
            if (!IntMap.ContainsKey(key))
                return false;
            value = IntMap[key];
            return true;
        }

        public bool TryGetValue(string key, out Color value, Color defaultValue = default)
        {
            value = defaultValue;
            if (!ColorMap.ContainsKey(key))
                return false;
            value = ColorMap[key];
            return true;
        }

        public bool TryGetValue(string key, out Vector2 value, Vector2 defaultValue = default)
        {
            value = defaultValue;
            if (!Vector2Map.ContainsKey(key))
                return false;
            value = Vector2Map[key];
            return true;
        }

        public bool TryGetValue(string key, out Vector3 value, Vector3 defaultValue = default)
        {
            value = defaultValue;
            if (!Vector3Map.ContainsKey(key))
                return false;
            value = Vector3Map[key];
            return true;
        }

        public bool TryGetValue(string key, out Vector4 value, Vector4 defaultValue = default)
        {
            value = defaultValue;
            if (!Vector4Map.ContainsKey(key))
                return false;
            value = Vector4Map[key];
            return true;
        }

        #endregion

        #region 数据操作2

        /// <summary>
        /// 设置自定义数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public void SetData<T>(string key, T data) where T : struct, IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            Delete(key);
            DataMap[key] = data;
            KeyToValueMap.Add(key, EasyValueType.None);
        }

        /// <summary>
        /// 判断是否有某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            if (TryGetMap(key, out var map))
                return map.Contains(key);
            return false;
        }

        /// <summary>
        /// 判断是否有某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public bool HasKey(string key, EasyValueType valueType)
        {
            if (TryGetMap(valueType, out var map))
                return map.Contains(key);
            return false;
        }

        /// <summary>
        /// 尝试删除某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryDelete<T>(string key, T data = default)
        {
            IDictionary map = GetMap(data);
            if (map is null)
                return false;
            map.Remove(key);
            return true;
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="key"></param>
        public void Delete(string key)
        {
            IntMap.Remove(key);
            FloatMap.Remove(key);
            BoolMap.Remove(key);
            StringMap.Remove(key);
            DataMap.Remove(key);
            ColorMap.Remove(key);
            Vector2Map.Remove(key);
            Vector3Map.Remove(key);
            Vector4Map.Remove(key);
            KeyToValueMap.Remove(key);
        }

        /// <summary>
        /// 根据数据类型删除Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueType"></param>
        public void Delete(string key, EasyValueType valueType)
        {
            GetMap(valueType).Remove(key);
            KeyToValueMap.Remove(key);
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void DeleteAll()
        {
            IntMap.Clear();
            FloatMap.Clear();
            BoolMap.Clear();
            StringMap.Clear();
            DataMap.Clear();
            ColorMap.Clear();
            Vector2Map.Clear();
            Vector3Map.Clear();
            Vector4Map.Clear();
            KeyToValueMap.Clear();
        }

        /// <summary>
        /// 获得某个键存储的数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public EasyValueType GetValueType(string key)
        {
            if (IntMap.ContainsKey(key))
                return EasyValueType.Int;
            if (BoolMap.ContainsKey(key))
                return EasyValueType.Boolean;
            if (FloatMap.ContainsKey(key))
                return EasyValueType.Float;
            if (StringMap.ContainsKey(key))
                return EasyValueType.String;
            if (ColorMap.ContainsKey(key))
                return EasyValueType.Color;
            if (Vector2Map.ContainsKey(key))
                return EasyValueType.Vector2;
            if (Vector3Map.ContainsKey(key))
                return EasyValueType.Vector3;
            if (Vector4Map.ContainsKey(key))
                return EasyValueType.Vector4;
            return EasyValueType.None;
        }

        /// <summary>
        /// 读取自定义数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>(string key, T defaultData = default) where T : struct, IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            if (!DataMap.ContainsKey(key))
                return defaultData;
            else
                return DataMap[key] is T ? (T)DataMap[key] : default;
        }

        /// <summary>
        /// 尝试获取自定义数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>数据库中存有Key且和指定类型匹配则返回</returns>
        public bool TryGetData<T>(string key, out T value) where T : struct, IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            value = default;
            if (!DataMap.ContainsKey(key))
                return false;
            if (DataMap[key] is not T data)
                return false;
            value = data;
            return true;
        }

        private bool TryGetMap(EasyValueType valueType, out IDictionary map)
        {
            map = GetMap(valueType);
            return map is not null;
        }

        private bool TryGetMap<T>(out IDictionary map, T data = default)
        {
            map = GetMap(data);
            return map is not null;
        }

        private bool TryGetMap(string key, out IDictionary map)
        {
            map = GetMap(key);
            return map is not null;
        }

        private IDictionary GetMap(EasyValueType valueType)
        {
            IDictionary map = valueType switch
            {
                EasyValueType.None => null,
                EasyValueType.Float => FloatMap,
                EasyValueType.Int => IntMap,
                EasyValueType.Boolean => BoolMap,
                EasyValueType.String => StringMap,
                EasyValueType.Color => ColorMap,
                EasyValueType.Vector2 => Vector2Map,
                EasyValueType.Vector3 => Vector3Map,
                EasyValueType.Vector4 => Vector4Map,
                _ => null
            };
            return map;
        }

        private IDictionary GetMap(string key)
        {
            if (KeyToValueMap.TryGetValue(key, out var value))
                return GetMap(value);
            return null;
        }

        private IDictionary GetMap<T>(T data = default)
        {
            IDictionary map = data switch
            {
                float => FloatMap,
                string => StringMap,
                bool => BoolMap,
                int => IntMap,
                Color => ColorMap,
                Vector2 => Vector2Map,
                Vector3 => Vector3Map,
                Vector4 => Vector4Map,
                _ => null
            };
            return map;
        }

        #endregion

        /// <summary>
        /// 设置存储路径
        /// </summary>
        /// <param name="filePath"></param>
        public EasyDataContainer SetFilePath(string filePath)
        {
            FilePath = filePath;
            return this;
        }

        /// <summary>
        /// 保存存档
        /// </summary>
        public async void Save()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                Debug.LogError("保存路径不能为空");
                return;
            }
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
                string jsonValue = JsonConvert.SerializeObject(this, settings);
                await File.WriteAllTextAsync(FilePath, jsonValue, Encoding.UTF8);
                Debug.Log($"数据大小：{System.Text.Encoding.UTF8.GetByteCount(jsonValue)}");
                Debug.Log($"保存路径: {FilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"保存路径：{FilePath}\n保存错误");
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 加载存档
        /// </summary>
        public async void Load()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                Debug.LogError("读取路径不能为空");
                return;
            }
            try
            {
                string jsonValue = await File.ReadAllTextAsync(FilePath, Encoding.UTF8);
                EasyDataContainer container = JsonConvert.DeserializeObject<EasyDataContainer>(jsonValue);
                Debug.Log($"数据大小：{System.Text.Encoding.UTF8.GetByteCount(jsonValue)}");
                Debug.Log($"读取路径: {FilePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"读取路径：{FilePath}\n读取错误");
                Debug.LogError(e);
            }
        }

        private void LoadContainer(EasyDataContainer container)
        {
            FloatMap = container.FloatMap ?? DictionaryPool<string, float>.Get();
            BoolMap = container.BoolMap ?? DictionaryPool<string, bool>.Get();
            StringMap = container.StringMap ?? DictionaryPool<string, string>.Get();
            IntMap = container.IntMap ?? DictionaryPool<string, int>.Get();
            DataMap = container.DataMap ?? DictionaryPool<string, IData>.Get();
            ColorMap = container.ColorMap ?? DictionaryPool<string, Color>.Get();
            Vector2Map = container.Vector2Map ?? DictionaryPool<string, Vector2>.Get();
            Vector3Map = container.Vector3Map ?? DictionaryPool<string, Vector3>.Get();
            Vector4Map = container.Vector4Map ?? DictionaryPool<string, Vector4>.Get();
            KeyToValueMap = container.KeyToValueMap ?? InitKeyToValueMap();
        }

        private Dictionary<string, EasyValueType> InitKeyToValueMap()
        {
            var _keyToValueMap = DictionaryPool<string, EasyValueType>.Get();
            foreach (var key in FloatMap.Keys)
                _keyToValueMap[key] = EasyValueType.Float;
            foreach (var key in BoolMap.Keys)
                _keyToValueMap[key] = EasyValueType.Boolean;
            foreach (var key in IntMap.Keys)
                _keyToValueMap[key] = EasyValueType.Int;
            foreach (var key in DataMap.Keys)
                _keyToValueMap[key] = EasyValueType.None;
            foreach (var key in StringMap.Keys)
                _keyToValueMap[key] = EasyValueType.String;
            foreach (var key in ColorMap.Keys)
                _keyToValueMap[key] = EasyValueType.Color;
            foreach (var key in Vector2Map.Keys)
                _keyToValueMap[key] = EasyValueType.Vector2;
            foreach (var key in Vector3Map.Keys)
                _keyToValueMap[key] = EasyValueType.Vector3;
            foreach (var key in Vector4Map.Keys)
                _keyToValueMap[key] = EasyValueType.Vector4;
            return _keyToValueMap;
        }
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

        public static implicit operator TransformContainer(Transform transform)
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

    /// <summary>
    /// 自定义存储类型接口
    /// </summary>
    public interface IData
    {
    }

    public static partial class EasyData
    {
        private static Dictionary<string, IData> DataMap = new();

        /// <summary>
        /// 设置自定义数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetData<T>(string key, T data) where T : struct, IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            Delete(key);
            DataMap[key] = data;
            KeyToValueMap.Add(key, EasyValueType.None);
        }

        /// <summary>
        /// 读取自定义数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultData"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetData<T>(string key, T defaultData = default) where T : struct, IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            if (!DataMap.ContainsKey(key))
                return defaultData;
            else
                return DataMap[key] is T ? (T)DataMap[key] : default;
        }

        /// <summary>
        /// 尝试获取自定义数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>数据库中存有Key且和指定类型匹配则返回</returns>
        public static bool TryGetData<T>(string key, out T value) where T : struct, IData
        {
#if UNITY_EDITOR
            var type = typeof(T);
            if (!Attribute.IsDefined(type, typeof(SerializableAttribute)))
            {
                Debug.LogError($"数据类型：{type} 未添加[Serializable]标签");
            }
#endif
            value = default;
            if (!DataMap.ContainsKey(key))
                return false;
            if (DataMap[key] is not T data)
                return false;
            value = data;
            return true;
        }
    }

    public static partial class EasyData
    {
        private static Dictionary<string, EasyValueType> KeyToValueMap = new();

        //常用数据
        private static Dictionary<string, int> IntMap = new();
        private static Dictionary<string, float> FloatMap = new();
        private static Dictionary<string, bool> BoolMap = new();

        private static Dictionary<string, string> StringMap = new();

        //Unity数据
        private static Dictionary<string, Color> ColorMap = new();
        private static Dictionary<string, Vector2> Vector2Map = new();
        private static Dictionary<string, Vector3> Vector3Map = new();

        private static Dictionary<string, Vector4> Vector4Map = new();

        //未投放使用
        private static Dictionary<string, TransformContainer> _transformMap = new();
        private static Dictionary<string, RectTransformContainer> _rectTransformMap = new();

        /// <summary>
        /// 文件存储的路径
        /// </summary>
        private static string _defaultDataPath = $"{Application.dataPath}/{_fileName}";

        private static string _fileName = Application.identifier + ".json";

        /// <summary>
        /// 数据存储的回调
        /// </summary>
        public static event Action<DataSaveState> OnDataSave;

        /// <summary>
        /// 加载数据的回调
        /// </summary>
        public static event Action<DataSaveState> OnDataLoad;

        /// <summary>
        /// 数据加载和存储的异常回调
        /// </summary>
        public static event Action<Exception> OnThrowException;
    }

    //设置数据
    public static partial class EasyData
    {
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">数据</param>
        public static void SetValue(string key, float value)
        {
            Delete(key);
            FloatMap.Add(key, value);
        }

        public static void SetValue(string key, bool value)
        {
            Delete(key);
            BoolMap.Add(key, value);
        }

        public static void SetValue(string key, string value)
        {
            Delete(key);
            StringMap.Add(key, value);
        }

        public static void SetValue(string key, int value)
        {
            Delete(key);
            IntMap.Add(key, value);
        }

        public static void SetValue(string key, Color value)
        {
            Delete(key);
            ColorMap.Add(key, value);
        }

        public static void SetValue(string key, Vector2 value)
        {
            Delete(key);
            Vector2Map.Add(key, value);
        }

        public static void SetValue(string key, Vector3 value)
        {
            Delete(key);
            Vector3Map.Add(key, value);
        }

        public static void SetValue(string key, Vector4 value)
        {
            Delete(key);
            Vector4Map.Add(key, value);
        }
    }

    //尝试获取数据,如果数据不存在，则会使用默认数据，
    public static partial class EasyData
    {
        /// <summary>
        /// 尝试获取数据
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">读取数据</param>
        /// <param name="defaultValue">默认数据</param>
        /// <returns></returns>
        public static bool TryGetValue(string key, out float value, float defaultValue = default)
        {
            value = defaultValue;
            if (!FloatMap.ContainsKey(key))
                return false;
            value = FloatMap[key];
            return true;
        }

        public static bool TryGetValue(string key, out bool value, bool defaultValue = default)
        {
            value = defaultValue;
            if (!BoolMap.ContainsKey(key))
                return false;
            value = BoolMap[key];
            return true;
        }

        public static bool TryGetValue(string key, out string value, string defaultValue = default)
        {
            value = defaultValue;
            if (!StringMap.ContainsKey(key))
                return false;
            value = StringMap[key];
            return true;
        }

        public static bool TryGetValue(string key, out int value, int defaultValue = default)
        {
            value = defaultValue;
            if (!IntMap.ContainsKey(key))
                return false;
            value = IntMap[key];
            return true;
        }

        public static bool TryGetValue(string key, out Color value, Color defaultValue = default)
        {
            value = defaultValue;
            if (!ColorMap.ContainsKey(key))
                return false;
            value = ColorMap[key];
            return true;
        }

        public static bool TryGetValue(string key, out Vector2 value, Vector2 defaultValue = default)
        {
            value = defaultValue;
            if (!Vector2Map.ContainsKey(key))
                return false;
            value = Vector2Map[key];
            return true;
        }

        public static bool TryGetValue(string key, out Vector3 value, Vector3 defaultValue = default)
        {
            value = defaultValue;
            if (!Vector3Map.ContainsKey(key))
                return false;
            value = Vector3Map[key];
            return true;
        }

        public static bool TryGetValue(string key, out Vector4 value, Vector4 defaultValue = default)
        {
            value = defaultValue;
            if (!Vector4Map.ContainsKey(key))
                return false;
            value = Vector4Map[key];
            return true;
        }
    }


    //数据全局操作
    public static partial class EasyData
    {
        private static EasyDataContainer GetContainer()
        {
            EasyDataContainer container = new()
            {
                IntMap = IntMap,
                FloatMap = FloatMap,
                StringMap = StringMap,
                BoolMap = BoolMap,
                DataMap = DataMap,
                ColorMap = ColorMap,
                Vector2Map = Vector2Map,
                Vector3Map = Vector3Map,
                Vector4Map = Vector4Map,
                KeyToValueMap = KeyToValueMap
            };
            return container;
        }

        private static void LoadContainer(EasyDataContainer container)
        {
            DictionaryPool<string, float>.Release(FloatMap);
            DictionaryPool<string, bool>.Release(BoolMap);
            DictionaryPool<string, string>.Release(StringMap);
            DictionaryPool<string, int>.Release(IntMap);
            DictionaryPool<string, IData>.Release(DataMap);
            DictionaryPool<string, Color>.Release(ColorMap);
            DictionaryPool<string, Vector2>.Release(Vector2Map);
            DictionaryPool<string, Vector3>.Release(Vector3Map);
            DictionaryPool<string, Vector4>.Release(Vector4Map);
            DictionaryPool<string, EasyValueType>.Release(KeyToValueMap);
            FloatMap = container.FloatMap ?? DictionaryPool<string, float>.Get();
            BoolMap = container.BoolMap ?? DictionaryPool<string, bool>.Get();
            StringMap = container.StringMap ?? DictionaryPool<string, string>.Get();
            IntMap = container.IntMap ?? DictionaryPool<string, int>.Get();
            DataMap = container.DataMap ?? DictionaryPool<string, IData>.Get();
            ColorMap = container.ColorMap ?? DictionaryPool<string, Color>.Get();
            Vector2Map = container.Vector2Map ?? DictionaryPool<string, Vector2>.Get();
            Vector3Map = container.Vector3Map ?? DictionaryPool<string, Vector3>.Get();
            Vector4Map = container.Vector4Map ?? DictionaryPool<string, Vector4>.Get();
            KeyToValueMap = container.KeyToValueMap ?? InitKeyToValueMap();
        }

        private static Dictionary<string, EasyValueType> InitKeyToValueMap()
        {
            var _keyToValueMap = DictionaryPool<string, EasyValueType>.Get();
            foreach (var key in FloatMap.Keys)
                _keyToValueMap[key] = EasyValueType.Float;
            foreach (var key in BoolMap.Keys)
                _keyToValueMap[key] = EasyValueType.Boolean;
            foreach (var key in IntMap.Keys)
                _keyToValueMap[key] = EasyValueType.Int;
            foreach (var key in DataMap.Keys)
                _keyToValueMap[key] = EasyValueType.None;
            foreach (var key in StringMap.Keys)
                _keyToValueMap[key] = EasyValueType.String;
            foreach (var key in ColorMap.Keys)
                _keyToValueMap[key] = EasyValueType.Color;
            foreach (var key in Vector2Map.Keys)
                _keyToValueMap[key] = EasyValueType.Vector2;
            foreach (var key in Vector3Map.Keys)
                _keyToValueMap[key] = EasyValueType.Vector3;
            foreach (var key in Vector4Map.Keys)
                _keyToValueMap[key] = EasyValueType.Vector4;
            return _keyToValueMap;
        }

        /// <summary>
        /// 判断是否有某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            if (TryGetMap(key, out var map))
                return map.Contains(key);
            return false;
        }

        /// <summary>
        /// 判断是否有某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static bool HasKey(string key, EasyValueType valueType)
        {
            if (TryGetMap(valueType, out var map))
                return map.Contains(key);
            return false;
        }

        /// <summary>
        /// 尝试删除某个Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryDelete<T>(string key, T data = default)
        {
            IDictionary map = GetMap(data);
            if (map is null)
                return false;
            map.Remove(key);
            return true;
        }

        /// <summary>
        /// 删除Key
        /// </summary>
        /// <param name="key"></param>
        public static void Delete(string key)
        {
            IntMap.Remove(key);
            FloatMap.Remove(key);
            BoolMap.Remove(key);
            StringMap.Remove(key);
            DataMap.Remove(key);
            ColorMap.Remove(key);
            Vector2Map.Remove(key);
            Vector3Map.Remove(key);
            Vector4Map.Remove(key);
            KeyToValueMap.Remove(key);
        }

        /// <summary>
        /// 根据数据类型删除Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="valueType"></param>
        public static void Delete(string key, EasyValueType valueType)
        {
            GetMap(valueType).Remove(key);
            KeyToValueMap.Remove(key);
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public static void DeleteAll()
        {
            IntMap.Clear();
            FloatMap.Clear();
            BoolMap.Clear();
            StringMap.Clear();
            DataMap.Clear();
            ColorMap.Clear();
            Vector2Map.Clear();
            Vector3Map.Clear();
            Vector4Map.Clear();
            KeyToValueMap.Clear();
        }

        /// <summary>
        /// 获得某个键存储的数据类型
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static EasyValueType GetValueType(string key)
        {
            if (IntMap.ContainsKey(key))
                return EasyValueType.Int;
            if (BoolMap.ContainsKey(key))
                return EasyValueType.Boolean;
            if (FloatMap.ContainsKey(key))
                return EasyValueType.Float;
            if (StringMap.ContainsKey(key))
                return EasyValueType.String;
            if (ColorMap.ContainsKey(key))
                return EasyValueType.Color;
            if (Vector2Map.ContainsKey(key))
                return EasyValueType.Vector2;
            if (Vector3Map.ContainsKey(key))
                return EasyValueType.Vector3;
            if (Vector4Map.ContainsKey(key))
                return EasyValueType.Vector4;
            return EasyValueType.None;
        }
    }

    public static partial class EasyData
    {
        private static bool TryGetMap(EasyValueType valueType, out IDictionary map)
        {
            map = GetMap(valueType);
            return map is not null;
        }

        private static bool TryGetMap<T>(out IDictionary map, T data = default)
        {
            map = GetMap(data);
            return map is not null;
        }

        private static bool TryGetMap(string key, out IDictionary map)
        {
            map = GetMap(key);
            return map is not null;
        }

        private static IDictionary GetMap(EasyValueType valueType)
        {
            IDictionary map = valueType switch
            {
                EasyValueType.None => null,
                EasyValueType.Float => FloatMap,
                EasyValueType.Int => IntMap,
                EasyValueType.Boolean => BoolMap,
                EasyValueType.String => StringMap,
                EasyValueType.Color => ColorMap,
                EasyValueType.Vector2 => Vector2Map,
                EasyValueType.Vector3 => Vector3Map,
                EasyValueType.Vector4 => Vector4Map,
                _ => null
            };
            return map;
        }

        private static IDictionary GetMap(string key)
        {
            if (KeyToValueMap.TryGetValue(key, out var value))
                return GetMap(value);
            return null;
        }

        private static IDictionary GetMap<T>(T data = default)
        {
            IDictionary map = data switch
            {
                float => FloatMap,
                string => StringMap,
                bool => BoolMap,
                int => IntMap,
                Color => ColorMap,
                Vector2 => Vector2Map,
                Vector3 => Vector3Map,
                Vector4 => Vector4Map,
                _ => null
            };
            return map;
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

        /// <summary>
        /// 获取当前配置的存档位置
        /// </summary>
        /// <returns></returns>
        public static string GetDataPath()
        {
            return _defaultDataPath;
        }

        /// <summary>
        /// 指定路径保存数据
        /// </summary>
        /// <param name="path"></param>
        public static void Save(string path)
        {
            Save(default, path);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param>
        ///     <name>path</name>
        /// </param>
        /// <param name="callBack"></param>
        /// <param name="path"></param>
        public static async void Save(Action<DataSaveState, Exception> callBack = default, string path = default)
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
                string jsonValue = JsonConvert.SerializeObject(GetContainer(), settings);
                Debug.Log($"数据大小：{System.Text.Encoding.UTF8.GetByteCount(jsonValue)}");
                Debug.Log($"保存路径: {path}");
                await File.WriteAllTextAsync(path, jsonValue, Encoding.UTF8);
            }
            catch (Exception e)
            {
                OnDataSave?.Invoke(DataSaveState.Error);
                OnThrowException?.Invoke(e);
                Debug.LogError(e);
                callBack?.Invoke(DataSaveState.Error, e);
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
        public static async void Load(Action<DataSaveState, Exception> callBack = default, string path = default)
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
                EasyDataContainer container = JsonConvert.DeserializeObject<EasyDataContainer>(jsonValue);
                LoadContainer(container);
            }
            catch (Exception e)
            {
                OnDataLoad?.Invoke(DataSaveState.Error);
                OnThrowException?.Invoke(e);
                Debug.LogError(e);
                callBack?.Invoke(DataSaveState.Error, e);
            }

            OnDataLoad?.Invoke(DataSaveState.Success);
        }
#if UNITY_EDITOR
        public static (string, EasyValueType)[] GetAllKey()
        {
            List<(string, EasyValueType)> list = UnityEngine.Pool.ListPool<(string, EasyValueType)>.Get();
            foreach (var key in IntMap.Keys)
                list.Add((key, EasyValueType.Int));
            foreach (var key in FloatMap.Keys)
                list.Add((key, EasyValueType.Float));
            foreach (var key in StringMap.Keys)
                list.Add((key, EasyValueType.String));
            foreach (var key in BoolMap.Keys)
                list.Add((key, EasyValueType.Boolean));
            foreach (var key in ColorMap.Keys)
                list.Add((key, EasyValueType.Color));
            foreach (var key in Vector2Map.Keys)
                list.Add((key, EasyValueType.Vector2));
            foreach (var key in Vector3Map.Keys)
                list.Add((key, EasyValueType.Vector3));
            foreach (var key in Vector4Map.Keys)
                list.Add((key, EasyValueType.Vector4));
            (string, EasyValueType)[] values = list.ToArray();
            ListPool<(string, EasyValueType)>.Release(list);
            return values;
        }

        public static (string, EasyValueType)[] SearchKey(string keyValue,
            StringComparison comparison = StringComparison.Ordinal)
        {
            List<(string, EasyValueType)> list = ListPool<(string, EasyValueType)>.Get();

            foreach (var key in IntMap.Keys)
            {
                if ("Int".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.Int));
            }

            foreach (var key in FloatMap.Keys)
            {
                if ("Float".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.Float));
            }

            foreach (var key in StringMap.Keys)
            {
                if ("String".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.String));
            }

            foreach (var key in BoolMap.Keys)
            {
                if ("Bool".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.Boolean));
            }

            foreach (var key in ColorMap.Keys)
            {
                if ("Color".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.Color));
            }

            foreach (var key in Vector2Map.Keys)
            {
                if ("Vector2".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.Vector2));
            }

            foreach (var key in Vector3Map.Keys)
            {
                if ("Vector3".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.Vector3));
            }

            foreach (var key in Vector4Map.Keys)
            {
                if ("Vector4".Contains(keyValue, comparison) || key.Contains(keyValue, comparison))
                    list.Add((key, EasyValueType.Vector4));
            }

            (string, EasyValueType)[] values = list.ToArray();
            ListPool<(string, EasyValueType)>.Release(list);
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
            string[] keys = IntMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetInt(key, IntMap[key]);
            }

            keys = BoolMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetInt(key, BoolMap[key] ? 0 : 1);
            }

            keys = FloatMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetFloat(key, FloatMap[key]);
            }

            keys = StringMap.Keys.ToArray();
            foreach (var key in keys)
            {
                PlayerPrefs.SetString(key, StringMap[key]);
            }
        }
    }

    public enum DataSaveState
    {
        Success,
        Error
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
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            bool isPublic = member switch
            {
                FieldInfo f => f.IsPublic,
                PropertyInfo p => (!(p.SetMethod is null || !p.SetMethod.IsPublic) && p.GetGetMethod().IsPublic),
                _ => false
            };
            property.ShouldSerialize = _ => isPublic;
            return property;
        }
    }
}