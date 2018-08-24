using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaBuilder
	{
		private JsonReader _reader;

		private readonly IList<JsonSchema> _stack;

		private readonly JsonSchemaResolver _resolver;

		private JsonSchema _currentSchema;

		private JsonSchema CurrentSchema => _currentSchema;

		public JsonSchemaBuilder(JsonSchemaResolver resolver)
		{
			_stack = new List<JsonSchema>();
			_resolver = resolver;
		}

		private void Push(JsonSchema value)
		{
			_currentSchema = value;
			_stack.Add(value);
			_resolver.LoadedSchemas.Add(value);
		}

		private JsonSchema Pop()
		{
			JsonSchema currentSchema = _currentSchema;
			_stack.RemoveAt(_stack.Count - 1);
			_currentSchema = _stack.LastOrDefault();
			return currentSchema;
		}

		internal JsonSchema Parse(JsonReader reader)
		{
			_reader = reader;
			if (reader.TokenType == JsonToken.None)
			{
				_reader.Read();
			}
			return BuildSchema();
		}

		private JsonSchema BuildSchema()
		{
			if (_reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected StartObject while parsing schema object, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
			_reader.Read();
			if (_reader.TokenType == JsonToken.EndObject)
			{
				Push(new JsonSchema());
				return Pop();
			}
			string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
			_reader.Read();
			if (text == "$ref")
			{
				string text2 = (string)_reader.Value;
				while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
				{
					if (_reader.TokenType == JsonToken.StartObject)
					{
						throw new Exception("Found StartObject within the schema reference with the Id '{0}'".FormatWith(CultureInfo.InvariantCulture, text2));
					}
				}
				JsonSchema schema = _resolver.GetSchema(text2);
				if (schema == null)
				{
					throw new Exception("Could not resolve schema reference for Id '{0}'.".FormatWith(CultureInfo.InvariantCulture, text2));
				}
				return schema;
			}
			Push(new JsonSchema());
			ProcessSchemaProperty(text);
			while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
			{
				text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
				_reader.Read();
				ProcessSchemaProperty(text);
			}
			return Pop();
		}

		private void ProcessSchemaProperty(string propertyName)
		{
			if (propertyName != null)
			{
				if (_003C_003Ef__switch_0024map0 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(29);
					dictionary.Add("type", 0);
					dictionary.Add("id", 1);
					dictionary.Add("title", 2);
					dictionary.Add("description", 3);
					dictionary.Add("properties", 4);
					dictionary.Add("items", 5);
					dictionary.Add("additionalProperties", 6);
					dictionary.Add("patternProperties", 7);
					dictionary.Add("required", 8);
					dictionary.Add("requires", 9);
					dictionary.Add("identity", 10);
					dictionary.Add("minimum", 11);
					dictionary.Add("maximum", 12);
					dictionary.Add("exclusiveMinimum", 13);
					dictionary.Add("exclusiveMaximum", 14);
					dictionary.Add("maxLength", 15);
					dictionary.Add("minLength", 16);
					dictionary.Add("maxItems", 17);
					dictionary.Add("minItems", 18);
					dictionary.Add("divisibleBy", 19);
					dictionary.Add("disallow", 20);
					dictionary.Add("default", 21);
					dictionary.Add("hidden", 22);
					dictionary.Add("readonly", 23);
					dictionary.Add("format", 24);
					dictionary.Add("pattern", 25);
					dictionary.Add("options", 26);
					dictionary.Add("enum", 27);
					dictionary.Add("extends", 28);
					_003C_003Ef__switch_0024map0 = dictionary;
				}
				if (_003C_003Ef__switch_0024map0.TryGetValue(propertyName, out int value))
				{
					switch (value)
					{
					case 0:
						CurrentSchema.Type = ProcessType();
						return;
					case 1:
						CurrentSchema.Id = (string)_reader.Value;
						return;
					case 2:
						CurrentSchema.Title = (string)_reader.Value;
						return;
					case 3:
						CurrentSchema.Description = (string)_reader.Value;
						return;
					case 4:
						ProcessProperties();
						return;
					case 5:
						ProcessItems();
						return;
					case 6:
						ProcessAdditionalProperties();
						return;
					case 7:
						ProcessPatternProperties();
						return;
					case 8:
						CurrentSchema.Required = (bool)_reader.Value;
						return;
					case 9:
						CurrentSchema.Requires = (string)_reader.Value;
						return;
					case 10:
						ProcessIdentity();
						return;
					case 11:
						CurrentSchema.Minimum = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);
						return;
					case 12:
						CurrentSchema.Maximum = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);
						return;
					case 13:
						CurrentSchema.ExclusiveMinimum = (bool)_reader.Value;
						return;
					case 14:
						CurrentSchema.ExclusiveMaximum = (bool)_reader.Value;
						return;
					case 15:
						CurrentSchema.MaximumLength = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
						return;
					case 16:
						CurrentSchema.MinimumLength = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
						return;
					case 17:
						CurrentSchema.MaximumItems = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
						return;
					case 18:
						CurrentSchema.MinimumItems = Convert.ToInt32(_reader.Value, CultureInfo.InvariantCulture);
						return;
					case 19:
						CurrentSchema.DivisibleBy = Convert.ToDouble(_reader.Value, CultureInfo.InvariantCulture);
						return;
					case 20:
						CurrentSchema.Disallow = ProcessType();
						return;
					case 21:
						ProcessDefault();
						return;
					case 22:
						CurrentSchema.Hidden = (bool)_reader.Value;
						return;
					case 23:
						CurrentSchema.ReadOnly = (bool)_reader.Value;
						return;
					case 24:
						CurrentSchema.Format = (string)_reader.Value;
						return;
					case 25:
						CurrentSchema.Pattern = (string)_reader.Value;
						return;
					case 26:
						ProcessOptions();
						return;
					case 27:
						ProcessEnum();
						return;
					case 28:
						ProcessExtends();
						return;
					}
				}
			}
			_reader.Skip();
		}

		private void ProcessExtends()
		{
			CurrentSchema.Extends = BuildSchema();
		}

		private void ProcessEnum()
		{
			if (_reader.TokenType != JsonToken.StartArray)
			{
				throw new Exception("Expected StartArray token while parsing enum values, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
			CurrentSchema.Enum = new List<JToken>();
			while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
			{
				JToken item = JToken.ReadFrom(_reader);
				CurrentSchema.Enum.Add(item);
			}
		}

		private void ProcessOptions()
		{
			CurrentSchema.Options = new Dictionary<JToken, string>(new JTokenEqualityComparer());
			JsonToken tokenType = _reader.TokenType;
			if (tokenType == JsonToken.StartArray)
			{
				JToken jToken;
				while (true)
				{
					if (!_reader.Read() || _reader.TokenType == JsonToken.EndArray)
					{
						return;
					}
					if (_reader.TokenType != JsonToken.StartObject)
					{
						throw new Exception("Expect object token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
					}
					string value = null;
					jToken = null;
					while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
					{
						string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
						_reader.Read();
						switch (text)
						{
						case "value":
							jToken = JToken.ReadFrom(_reader);
							break;
						case "label":
							value = (string)_reader.Value;
							break;
						default:
							throw new Exception("Unexpected property in JSON schema option: {0}.".FormatWith(CultureInfo.InvariantCulture, text));
						}
					}
					if (jToken == null)
					{
						throw new Exception("No value specified for JSON schema option.");
					}
					if (CurrentSchema.Options.ContainsKey(jToken))
					{
						break;
					}
					CurrentSchema.Options.Add(jToken, value);
				}
				throw new Exception("Duplicate value in JSON schema option collection: {0}".FormatWith(CultureInfo.InvariantCulture, jToken));
			}
			throw new Exception("Expected array token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
		}

		private void ProcessDefault()
		{
			CurrentSchema.Default = JToken.ReadFrom(_reader);
		}

		private void ProcessIdentity()
		{
			CurrentSchema.Identity = new List<string>();
			switch (_reader.TokenType)
			{
			case JsonToken.String:
				CurrentSchema.Identity.Add(_reader.Value.ToString());
				break;
			case JsonToken.StartArray:
				while (true)
				{
					if (!_reader.Read() || _reader.TokenType == JsonToken.EndArray)
					{
						return;
					}
					if (_reader.TokenType != JsonToken.String)
					{
						break;
					}
					CurrentSchema.Identity.Add(_reader.Value.ToString());
				}
				throw new Exception("Exception JSON property name string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			default:
				throw new Exception("Expected array or JSON property name string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
		}

		private void ProcessAdditionalProperties()
		{
			if (_reader.TokenType == JsonToken.Boolean)
			{
				CurrentSchema.AllowAdditionalProperties = (bool)_reader.Value;
			}
			else
			{
				CurrentSchema.AdditionalProperties = BuildSchema();
			}
		}

		private void ProcessPatternProperties()
		{
			Dictionary<string, JsonSchema> dictionary = new Dictionary<string, JsonSchema>();
			if (_reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected start object token.");
			}
			while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
			{
				string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
				_reader.Read();
				if (dictionary.ContainsKey(text))
				{
					throw new Exception("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, text));
				}
				dictionary.Add(text, BuildSchema());
			}
			CurrentSchema.PatternProperties = dictionary;
		}

		private void ProcessItems()
		{
			CurrentSchema.Items = new List<JsonSchema>();
			switch (_reader.TokenType)
			{
			case JsonToken.StartObject:
				CurrentSchema.Items.Add(BuildSchema());
				break;
			case JsonToken.StartArray:
				while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
				{
					CurrentSchema.Items.Add(BuildSchema());
				}
				break;
			default:
				throw new Exception("Expected array or JSON schema object token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
		}

		private void ProcessProperties()
		{
			IDictionary<string, JsonSchema> dictionary = new Dictionary<string, JsonSchema>();
			if (_reader.TokenType != JsonToken.StartObject)
			{
				throw new Exception("Expected StartObject token while parsing schema properties, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
			while (_reader.Read() && _reader.TokenType != JsonToken.EndObject)
			{
				string text = Convert.ToString(_reader.Value, CultureInfo.InvariantCulture);
				_reader.Read();
				if (dictionary.ContainsKey(text))
				{
					throw new Exception("Property {0} has already been defined in schema.".FormatWith(CultureInfo.InvariantCulture, text));
				}
				dictionary.Add(text, BuildSchema());
			}
			CurrentSchema.Properties = dictionary;
		}

		private JsonSchemaType? ProcessType()
		{
			switch (_reader.TokenType)
			{
			case JsonToken.String:
				return MapType(_reader.Value.ToString());
			case JsonToken.StartArray:
			{
				JsonSchemaType? result = JsonSchemaType.None;
				while (_reader.Read() && _reader.TokenType != JsonToken.EndArray)
				{
					if (_reader.TokenType != JsonToken.String)
					{
						throw new Exception("Exception JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
					}
					result = ((!result.HasValue) ? null : new JsonSchemaType?(result.GetValueOrDefault() | MapType(_reader.Value.ToString())));
				}
				return result;
			}
			default:
				throw new Exception("Expected array or JSON schema type string token, got {0}.".FormatWith(CultureInfo.InvariantCulture, _reader.TokenType));
			}
		}

		internal static JsonSchemaType MapType(string type)
		{
			if (!JsonSchemaConstants.JsonSchemaTypeMapping.TryGetValue(type, out JsonSchemaType value))
			{
				throw new Exception("Invalid JSON schema type: {0}".FormatWith(CultureInfo.InvariantCulture, type));
			}
			return value;
		}

		internal static string MapType(JsonSchemaType type)
		{
			return JsonSchemaConstants.JsonSchemaTypeMapping.Single((KeyValuePair<string, JsonSchemaType> kv) => kv.Value == type).Key;
		}
	}
}
