using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class JSONDisplayer : MonoBehaviour{

	public GameObject textPrefab;
	public GameObject itemTextPrefab;
	public GameObject rowPrefab;
	public GameObject rowsObject;

	public float waitTime;

	public Text title;

	private GameObject headers;
	private IEnumerator updateCoroutine;



	[Serializable]
	public class JsonObject{
		public string Title;
		public List<string> ColumnHeaders;
		public List<Dictionary<string,string>> Data;
	}	

	[SerializeField]
	private JsonObject jsonObject;

	public string jsonFileName;

	// initialization
	void Start () {

		updateCoroutine = WaitAndUpdate(waitTime);
		StartCoroutine(updateCoroutine);
	}

	//I decided to use a coroutine instead of the update because 
	//i don't think it's a good idea to be rading a file every frame

	IEnumerator WaitAndUpdate(float waitTime){
		while(true){
			string json = File.ReadAllText(jsonFileName);
			//unity's Json Utility wasn't useful to this type of data,
			// so i used Json.net to do the deserialization
			jsonObject = JsonConvert.DeserializeObject<JsonObject>(json);
			DisplayData(jsonObject);
			yield return new WaitForSeconds(waitTime);

		}
	}


	//display the data
	void DisplayData(JsonObject jOb){
		title.text = jsonObject.Title;

		//destroy any old list if any before displaying the new one
		if (headers != null) 
		{
			GameObject.Destroy(headers);
			GameObject[] objects = GameObject.FindGameObjectsWithTag("rows");
			for(int i=0; i<objects.Length ;i++){
				GameObject.Destroy(objects[i]);
			}
		}

		//display the new list
		headers = Instantiate(rowPrefab, rowsObject.transform);
		foreach(string column in jOb.ColumnHeaders)
		{
			GameObject header  = Instantiate(textPrefab, headers.transform);
			header.GetComponent<Text>().text = column;

		}

		foreach (Dictionary<string,string> row in jOb.Data) 
		{
			GameObject newRow = Instantiate(rowPrefab, rowsObject.transform);
			foreach(string column in jOb.ColumnHeaders)
			{
				GameObject item = Instantiate(itemTextPrefab, newRow.transform);
				item.GetComponent<Text>().text = row[column];
			}
		}

	}

}
