﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD.Entities
{
    internal class ScormDAO : IDAO
    {
        private String _password;
        private Scorm _persist;

        public Scorm GetSaved
        {
            get { return this._persist; }
        }

        public ScormDAO(string contextPassword)
        {
                this._password = contextPassword;
        }

        public ScormDAO(string contextPassword, Entities.Scorm toPersist)
        {
            this._persist = toPersist;
            this._password = contextPassword;
        }

        void IDAO.Save()
        {

            if (this._persist != null)
            {
                RDModelContainer model = Context.SetContext(_password).model;
                model.Scorms.Add(this._persist);
                model.SaveChanges();
            }

        }

        void IDAO.Update()
        {
            if (this._persist != null)
            {
                RDModelContainer model = Context.SetContext(_password).model;
                Scorm scorm = model.Scorms.Single(s => s.Id.Equals(this._persist.Id));
                scorm.Credit = this._persist.Credit;
                scorm.DataMasteryScore = this._persist.DataMasteryScore;
                scorm.Entry = this._persist.Entry;
                scorm.Exit = this._persist.Exit;
                scorm.LaunchData = this._persist.LaunchData;
                scorm.LessonLocation = this._persist.LessonLocation;
                scorm.ScoreMax = this._persist.ScoreMax;
                scorm.ScoreMin = this._persist.ScoreMin;
                scorm.ScoreRaw = this._persist.ScoreRaw;
                scorm.SessionTime = this._persist.SessionTime;
                scorm.SuspendData = this._persist.SuspendData;
                scorm.TotalTime = this._persist.TotalTime;
                scorm.Version = this._persist.Version;
                model.SaveChanges();
            }
        }

        void IDAO.Delete()
        {
            throw new NotImplementedException();
        }
    }
}
