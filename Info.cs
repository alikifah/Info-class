﻿using System;
using System.IO;
using System.Reflection;
using ExtensionMethods;
namespace ExtensionMethods
{
	public static class BinExtensions
   {
//################# string array
   public static void WriteStringArray( this BinaryWriter w, string[] arr ){
	   w.Write(arr.Length); // save length
	   foreach(string s in arr)
				w.Write(s);
	}
   public static string[] ReadStringArray( this BinaryReader r){
	   int n = r.ReadInt32(); // load length of the array
	   string[] d = new string[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadString();
		return d;
	}
//################# int array	
   public static void WriteIntArray( this BinaryWriter w, int[] arr ){
	   w.Write(arr.Length); // save length
	   foreach(int s in arr)
				w.Write(s);
	}
   public static int[] ReadIntArray( this BinaryReader r){
	   int n = r.ReadInt32();// load length of the array
	   int[] d = new int[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadInt32();
		return d;
	}
//################## byte array	
   public static void WriteByteArray( this BinaryWriter w, byte[] arr ){
	   w.Write(arr.Length);
	   foreach(byte s in arr)
				w.Write(s);
	}
   public static byte[] ReadByteArray( this BinaryReader r){
	   int n = r.ReadInt32();
	   byte[] d = new byte[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadByte();
		return d;
	}
//################## float array	
   public static void WriteFloatArray( this BinaryWriter w, float[] arr ){
	   w.Write(arr.Length);
	   foreach(float s in arr)
				w.Write(s);
	}
   public static float[] ReadFloatArray( this BinaryReader r){
	   int n = r.ReadInt32();
	   float[] d = new float[n];
	   for(int i = 0; i<n;i++ )
			d[i] = r.ReadByte();
		return d;
	}
   }
}

// Information holder class
public abstract class Info
{
  private FieldInfo[] myFieldInfo;
  public Info(){	 
    get_info(); // get public field member in Array
  }
  public Info(byte[] b){	 
    get_info(); // get public field member in Array
	Load(b);	
  }
	private byte[] FileToBinary(string filepath) {    
		FileStream stream = File.OpenRead( filepath );
		byte[] fileBytes= new byte[stream.Length];
		stream.Read(fileBytes, 0, fileBytes.Length);
		stream.Close();
		return fileBytes;
   }
   
	private void BinaryToFile( byte[] fileBytes, string filepath) {
        FileStream stream = File.OpenWrite( filepath );
		stream.Write(fileBytes, 0, fileBytes.Length);
		stream.Close();
   }
   
   public void Load(byte[] data) {
      using (MemoryStream m = new MemoryStream(data)) {
         using ( BinaryReader reader = new BinaryReader(m)) {
      read(reader);
         }
      }
   }
   public void Load(string filePath) {
		Load(FileToBinary(filePath));
   }
   
   public void Save(string filePath) {
	   BinaryToFile(Serialize(), filePath );
   }

   public byte[] Serialize() {
      using (MemoryStream m = new MemoryStream()) {
         using ( BinaryWriter writer = new BinaryWriter(m)) {
       write(writer);
         }
         return m.ToArray();
      }
   }
   private void write(BinaryWriter writer){
	// write class name for sake of compatibility check
     writer.Write( this.GetType().ToString() ); 
     // write content
	 foreach(var t in myFieldInfo){
       var v = t.GetValue(this);
      if (v == null ) continue;
      if (v.GetType() == typeof(string))
        writer.Write( (string)v );
      else if (v.GetType() == typeof(string[]))
        writer.WriteStringArray( (string[])v );
      else if (v.GetType() == typeof(int))
        writer.Write( (int)v);
      else if (v.GetType() == typeof(float))
        writer.Write( (float)v );  
      else if (v.GetType() == typeof(bool))
        writer.Write( (bool)v );
      else if (v.GetType() == typeof(byte))
        writer.Write( (byte)v );
      else if (v.GetType() == typeof(char))
        writer.Write( (char)v );  
      else if (v.GetType() == typeof(double))
        writer.Write( (double)v);
      else if (v.GetType() == typeof(byte[]))
        writer.WriteByteArray( (byte[])v);	
      else if (v.GetType() == typeof(int[]))
        writer.WriteIntArray( (int[])v);	
      else if (v.GetType() == typeof(float[]))
        writer.WriteFloatArray( (float[])v);
	
     }
   }
   private void read(BinaryReader reader){
    // read class name and compare it with the class of the byte array
	if (reader.ReadString() != this.GetType().ToString() ){
		//throw new ArgumentException("Type compatibility error!"  );
		return;
	}
	// read content
    foreach(var t in myFieldInfo){
      var v = t.GetValue(this);
      if (v == null ) continue;
      if (v.GetType() == typeof(string[]))
        t.SetValue(this, reader.ReadStringArray()); 
      else if (v.GetType() == typeof(string))
        t.SetValue(this, reader.ReadString()); 
      else if (v.GetType() == typeof(int))
        t.SetValue(this, reader.ReadInt32());  
      else if (v.GetType() == typeof(bool))
        t.SetValue(this, reader.ReadBoolean());
      else if (v.GetType() == typeof(float))
        t.SetValue(this, reader.ReadSingle());
      else if (v.GetType() == typeof(byte))
        t.SetValue(this, reader.ReadByte());
      else if (v.GetType() == typeof(char))
        t.SetValue(this, reader.ReadChar());
      else if (v.GetType() == typeof(double))
        t.SetValue(this, reader.ReadDouble());
      else if (v.GetType() == typeof(byte[]))
        t.SetValue(this, reader.ReadByteArray());	
      else if (v.GetType() == typeof(int[]))
        t.SetValue(this, reader.ReadIntArray());
      else if (v.GetType() == typeof(float[]))
        t.SetValue(this, reader.ReadFloatArray());	
	
    }
   }
   public void Reset(){
    foreach(var t in myFieldInfo){
      var v = t.GetValue(this);
      if (v == null ) continue;
      if (v.GetType() == typeof(string[]))
        t.SetValue(this, null); 
      else if (v.GetType() == typeof(string))
        t.SetValue(this, ""); 
      else if (v.GetType() == typeof(int))
        t.SetValue(this, 0);  
      else if (v.GetType() == typeof(bool))
        t.SetValue(this, false);
      else if (v.GetType() == typeof(float))
        t.SetValue(this, default(float));
      else if (v.GetType() == typeof(byte))
        t.SetValue(this, 0);
      else if (v.GetType() == typeof(char))
        t.SetValue(this, '\0');
      else if (v.GetType() == typeof(double))
        t.SetValue(this, default(double));
      else if (v.GetType() == typeof(byte[]))
        t.SetValue(this, null);	
      else if (v.GetType() == typeof(int[]))
        t.SetValue(this, null);
      else if (v.GetType() == typeof(float[]))
        t.SetValue(this, null);
    }
   }
   
  private void get_info(){
    Type myType = this.GetType();
    myFieldInfo = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);
    correctNull();	 
  }
  private void correctNull(){
	foreach(var t in myFieldInfo){
		if (t.GetValue(this) == null){
			//Console.Write( t.Name + " is null" );
			try{
				t.SetValue(this,"");
			}
			catch(ArgumentException ex1){
				try{
					t.SetValue(this,new int[]{});
				}
				catch(ArgumentException ex2){
					try{
						t.SetValue(this,new string[]{});
					}
					catch(ArgumentException ex3){
						try{
							t.SetValue(this,new byte[]{});
						}
						catch(ArgumentException ex4){				
							try{
								t.SetValue(this,new float[]{});
							}
							catch(ArgumentException ex5){
			
				
				
				
							}	
						}			
					}			
				}
			}
		}
	}
  }
  
    
    




