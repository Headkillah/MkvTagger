﻿#region Copyright (C) 2007-2013 Team MediaPortal

/*
    Copyright (C) 2007-2013 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Matroska
{
  [XmlRoot("Tags")]
  public class MatroskaTags
  {
    [XmlElement("Tag")]
    public List<Tag> TagList { get; set; }

    public MatroskaTags()
    {
      TagList = new List<Tag>();
      Series = new SeriesTag(this);
      Movie = new MovieTag(this);
      MusicVideo = new MusicVideoTag(this);
    }

    #region tag interpreters

    [XmlIgnore]
    public SeriesTag Series { get; set; }
    [XmlIgnore]
    public MovieTag Movie { get; set; }
    [XmlIgnore]
    public MusicVideoTag MusicVideo { get; set; }

    #endregion tag interpreters

    #region helper methods

    public Tag GetTag(int targetTypeValue)
    {
      return TagList.FirstOrDefault(t => t.Targets.TargetTypeValue == targetTypeValue);
    }

    public Tag GetOrAddTag(int targetTypeValue)
    {
      Tag tag = TagList.FirstOrDefault(t => t.Targets.TargetTypeValue == targetTypeValue);
      if (!ReferenceEquals(tag, null))
        return tag;

      tag = new Tag(targetTypeValue);
      TagList.Add(tag);

      return tag;
    }

    public void RemoveSpecificSimpleChild(XElement tagOrSimple, string name)
    {
      while (true)
      {
        IEnumerable<XElement> simpleCollection = tagOrSimple.Elements("Simple").Where(simple =>
        {
          XElement element = simple.Element("Name");
          return element != null && element.Value == name;
        });

        if (!simpleCollection.Any())
          break;

        foreach (XElement xElement in simpleCollection)
          xElement.Remove();
      }
    }

    public void AddSimpleCollection(XElement tagOrSimple, string name, IEnumerable values)
    {
      foreach (object value in values)
      {
        XElement simpleElement = new XElement("Simple");
        simpleElement.SetElementValue("Name", name);
        simpleElement.SetElementValue("String", value);

        tagOrSimple.Add(simpleElement);
      }
    }

    public void ReadTags(string filename)
    {
      MatroskaTags tags = MatroskaLoader.ReadTag(filename);
      if (tags != null)
        TagList = tags.TagList;
    }

    #endregion helper methods
  }
}