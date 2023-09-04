using System;
using System.Collections.Generic;
using T2TFramework.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
public class EasyDataWindow : EditorWindow
{
    public static Color SelectColor = new Color32(242, 137, 193, 200);
    public static Color DeSelectColor = new Color(1, 1, 1, 0);
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    [SerializeField]
    private EasyDataSetting Setting;
    private TextField KeyField;
    private FloatField FloatField;
    private Toggle BoolField;
    private IntegerField IntField;
    private TextField StringField;
    private ColorField ColorField;
    private Vector2Field Vector2Field;
    private Vector3Field Vector3Field;
    private Vector4Field Vector4Field;
    private EnumField SaveValueField;
    private Label FilePathLabel;
    private ToolbarPopupSearchField SearchKey;
    private TextField SearchKeyInput;
    private ListView KeyList;
    
    [MenuItem("EasyData/存档编辑器",false)]
    public static void ShowExample()
    {
        EasyDataWindow wnd = GetWindow<EasyDataWindow>();
        wnd.titleContent = new GUIContent("EasyDataWindow");
    }
    [MenuItem("EasyData/Test/CreatSampleData",false)]
    public static void CreatSampleData()
    {
        EasyData.SetValue("TestBool",true);
        EasyData.SetValue("TestFloat",11.3f);
        EasyData.SetValue("TestColor",Color.cyan);
        EasyData.SetValue("TestInt",114514);
        EasyData.SetValue("TestString","TESTString");
        EasyData.SetValue("TestVector2",new Vector2(1.4f,44));
        EasyData.SetValue("TestVector2_2",new Vector2(1.4f,4.4f));
        EasyData.SetValue("TestVector4",new Vector4(1.4f,4.4f,23,114.514f));
    }
    [MenuItem("EasyData/Test/SaveSampleData",false)]
    public static void SaveData()
    {
        EasyData.SetDataPath($"{Application.dataPath}/EasyData/TestData.json");
        EasyData.Save();
        Debug.Log("保存存档："+EasyData.GetDataPath());
    }
    [MenuItem("EasyData/QuickTool/ClearData",false)]
    public static void ClearData()
    {
        EasyData.DeleteAll();
    }

    private void Update()
    {
        Refresh();
    }

    public void CreateGUI()
    {
        m_VisualTreeAsset.CloneTree(rootVisualElement);
        var value = rootVisualElement.Q<VisualElement>("Value");
        var key = rootVisualElement.Q<VisualElement>("Key");
        var file = rootVisualElement.Q<VisualElement>("File");

        FilePathLabel = file.Q<VisualElement>("FilePathElement").Q<Label>("FilePath");
        if(Setting is null)
            Debug.Log("SoSetting is Null");
        else
        {
            FilePathLabel.text = Setting.filePath;
            EasyData.Load(Setting.filePath);
        }
        KeyField = value.Q<TextField>("KeyName");
        KeyField.RegisterCallback<InputEvent>((evt) =>
        {
            if (String.IsNullOrEmpty(evt.newData))
            {
                InitValue();
            }
            else
            {
                LoadValue(evt.newData);
            }
        });
        FloatField = value.Q<FloatField>("FloatValue");
        BoolField = value.Q<Toggle>("BoolValue");
        IntField = value.Q<IntegerField>("IntValue");
        StringField = value.Q<TextField>("StringValue");
        ColorField = value.Q<ColorField>("ColorValue");
        Vector2Field = value.Q<Vector2Field>("Vector2Value");
        Vector3Field = value.Q<Vector3Field>("Vector3Value");
        Vector4Field = value.Q<Vector4Field>("Vector4Value");
        SaveValueField = value.Q<EnumField>("SaveValueEnum");
        #region Button
        value.Q<Button>("LoadValue").clickable = new Clickable(LoadValue);
        value.Q<Button>("SaveValue").clickable = new Clickable(SaveValue);
        value.Q<Button>("DeleteKey").clickable = new Clickable(Delete);
        SearchKey = key.Q<ToolbarPopupSearchField>("SearchKey");
        KeyList = key.Q<ListView>("KeyList");
        #endregion
        
        file.Q<Button>("SaveFile").clickable = new Clickable(() =>
        {
            string filePath = GetFilePath();
            if(string.IsNullOrEmpty(filePath))
                return;
            EasyData.Save(filePath);
        });
        file.Q<Button>("LoadFile").clickable = new Clickable(() =>
        { 
            string filePath = EditorUtility.OpenFilePanel("选择文件", Application.dataPath,"json");
            if(string.IsNullOrEmpty(filePath))
                return;
            Setting.filePath = filePath;
            FilePathLabel.text = Setting.filePath;

            EasyData.Load(filePath);
        });
        file.Q<Button>("DeleteData").clickable = new Clickable(EasyData.DeleteAll);
        
        SearchKeyInput = SearchKey.Q<TextField>();

        InitValue();
    }
    public void Delete()
    {
        string key = KeyField.value;
        EasyData.Delete(key);
        KeyField.value = string.Empty;
        InitValue();
    }
    
    public void LoadValue(string key)
    {
        InitValue();
        if (!EasyData.HasKey(key))
        {
            // Debug.LogWarning($"[EasyDataEditorWindow]:Key {key} 不存在对应的Value");
            return;
        }
        var valueType = EasyData.GetValueType(key);
        if (valueType == EasyValueType.None)
        {
            // Debug.LogWarning($"[EasyDataEditorWindow]:Value类型为IData或者不存在，不支持更改");
            return;
        }

        switch (valueType)
        {
            case EasyValueType.Int:
                int intValue = default;
                EasyData.TryGetValue(key, out intValue);
                IntField.value = intValue;
                IntField.labelElement.style.backgroundColor = SelectColor;
                break;
            case EasyValueType.Float:
                float floatValue = default;
                EasyData.TryGetValue(key, out floatValue);
                FloatField.value = floatValue;
                FloatField.labelElement.style.backgroundColor = SelectColor;
                break;
            case EasyValueType.String:
                string stringValue = default;
                EasyData.TryGetValue(key, out stringValue);
                StringField.value = stringValue;
                StringField.labelElement.style.backgroundColor = SelectColor;

                break;
            case EasyValueType.Boolean:
                bool boolValue = default;
                EasyData.TryGetValue(key, out boolValue);
                BoolField.value = boolValue;
                BoolField.labelElement.style.backgroundColor = SelectColor;

                break;
            case EasyValueType.Color:
                Color colorValue = default;
                EasyData.TryGetValue(key, out colorValue);
                ColorField.value = colorValue;
                ColorField.labelElement.style.backgroundColor = SelectColor;

                break;
            case EasyValueType.Vector2:
                Vector2 vector2 = default;
                EasyData.TryGetValue(key, out vector2);
                Vector2Field.value = vector2;
                Vector2Field.labelElement.style.backgroundColor = SelectColor;

                break;
            case EasyValueType.Vector3:
                Vector3 vector3 = default;
                EasyData.TryGetValue(key, out vector3);
                Vector3Field.value = vector3;
                Vector3Field.labelElement.style.backgroundColor = SelectColor;

                break;
            case EasyValueType.Vector4:
                Vector4 vector4 = default;
                EasyData.TryGetValue(key, out vector4);
                Vector4Field.value = vector4;
                Vector4Field.labelElement.style.backgroundColor = SelectColor;
                break;
            default:
                break;
        }
        SaveValueField.value = valueType;
    }
    private void LoadValue()
    {
        string key = KeyField.value;
        LoadValue(key);
    }
    public void SaveValue()
    {
        Enum type = SaveValueField.value;
        string key = KeyField.value;
        if (type is EasyValueType easyValueType)
        {
            switch (easyValueType)
            {
                case EasyValueType.Boolean:
                    EasyData.SetValue(key,BoolField.value);
                    break;
                case EasyValueType.Float:
                    EasyData.SetValue(key,FloatField.value);
                    break;
                case EasyValueType.Int:
                    EasyData.SetValue(key,IntField.value);
                    break;
                case EasyValueType.String:
                    EasyData.SetValue(key,StringField.value);
                    break;
                case EasyValueType.Color:
                    EasyData.SetValue(key,ColorField.value);
                    break;
                case EasyValueType.Vector2:
                    EasyData.SetValue(key,Vector2Field.value);
                    break;
                case EasyValueType.Vector3:
                    EasyData.SetValue(key,Vector3Field.value);
                    break;
                case EasyValueType.Vector4:
                    EasyData.SetValue(key,Vector3Field.value);
                    break;
                default:
                    break;
            }
        }
        LoadValue();
    }
    private void InitValue()
    {
        FloatField.value = default;
        BoolField.value = default;
        IntField.value = default;
        StringField.value = default;
        ColorField.value = default;
        Vector2Field.value = default;
        Vector3Field.value = default;
        Vector4Field.value = default;
        FloatField.labelElement.style.backgroundColor = DeSelectColor;
        BoolField.labelElement.style.backgroundColor = DeSelectColor;
        IntField.labelElement.style.backgroundColor = DeSelectColor;
        StringField.labelElement.style.backgroundColor = DeSelectColor;
        ColorField.labelElement.style.backgroundColor = DeSelectColor;
        Vector2Field.labelElement.style.backgroundColor = DeSelectColor;
        Vector3Field.labelElement.style.backgroundColor = DeSelectColor;
        Vector4Field.labelElement.style.backgroundColor = DeSelectColor;
    }

    private void Refresh()
    {
        LoadKeyList(SearchKeyInput.value);
    }

    private string GetFilePath()
    {
        return Setting.filePath;
    }

    private List<(string,EasyValueType)> keyList = new();
    private void LoadKeyList(string key)
    {
        List< (string,EasyValueType)> dataList = ListPool< (string,EasyValueType)>.Get();
        if (string.IsNullOrEmpty(key))
        {
            dataList.AddRange(EasyData.GetAllKey());
        }
        else
        {
            dataList.AddRange(EasyData.SearchKey(key,StringComparison.OrdinalIgnoreCase));
        }
        
        KeyList.Clear();
        ListPool<(string,EasyValueType)>.Release(keyList);
        keyList = dataList;
        KeyList.itemsSource = dataList;
        KeyList.makeItem = () =>
        {
            VisualElement keyElement = new();
            Label key = new Label();
            key.name = "key";
            key.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
            Label valueType = new Label();
            valueType.name = "type";
            valueType.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleRight);

            keyElement.Add(key);
            keyElement.Add(valueType);
            keyElement.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            keyElement.style.justifyContent = new StyleEnum<Justify>(Justify.SpaceBetween);
            keyElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                KeyField.value = key.text;
                LoadValue();
            });
            
            return keyElement;
        };
        KeyList.bindItem = (element, index) =>
        {
            string key = keyList[index].Item1;
            string valueType = keyList[index].Item2.ToString();
            element.Q<Label>("key").text = key;
            element.Q<Label>("type").text = valueType;
        };
        KeyList.Rebuild();
    }
}
