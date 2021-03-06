﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kOS
{
    public class Node : Structure
    {
        ManeuverNode nodeRef;
        Vessel vesselRef;
        public double UT;
        public double Pro;
        public double RadOut;
        public double Norm;

        public static Dictionary<ManeuverNode, Node> NodeLookup = new Dictionary<ManeuverNode, Node>();

        public Node(double ut, double radialOut, double normal, double prograde)
        {
            this.UT = ut;
            this.Pro = prograde;
            this.RadOut = radialOut;
            this.Norm = normal;
        }

        public Node(Vessel v, ManeuverNode existingNode)
        {
            nodeRef = existingNode;
            vesselRef = v;
            NodeLookup.Add(existingNode, this);

            updateValues();
        }

        public static Node FromExisting(Vessel v, ManeuverNode existingNode)
        {
            if (NodeLookup.ContainsKey(existingNode)) return NodeLookup[existingNode];
            
            return new Node(v, existingNode);
        }
        
        public void AddToVessel(Vessel v)
        {
            if (nodeRef != null) throw new kOSException("Node has already been added");

            vesselRef = v;
            nodeRef = v.patchedConicSolver.AddManeuverNode(UT);

            UpdateNodeDeltaV();

            v.patchedConicSolver.UpdateFlightPlan();

            NodeLookup.Add(nodeRef, this);
        }

        public void UpdateAll()
        {
            UpdateNodeDeltaV();
            nodeRef.OnGizmoUpdated(new Vector3d(RadOut, Norm, Pro), UT);
        }

        private void UpdateNodeDeltaV()
        {
            if (nodeRef != null)
            {
                Vector3d dv = new Vector3d(RadOut, Norm, Pro);
                nodeRef.DeltaV = dv;
            }
        }

        public void CheckNodeRef()
        {
            if (nodeRef == null)
            {
                throw new kOSException("Must attach node first");
            }
        }

        public Vector GetBurnVector()
        {
            CheckNodeRef();

            return new Vector(nodeRef.GetBurnVector(vesselRef.GetOrbit()));
        }

        private void updateValues()
        {
            // If this node is attached, and the values on the attached node have chaged, I need to reflect that
            if (nodeRef != null)
            {
                UT = nodeRef.UT;

                RadOut = nodeRef.DeltaV.x;
                Norm = nodeRef.DeltaV.y;
                Pro = nodeRef.DeltaV.z;
            }
        }
                
        public override object GetSuffix(string suffixName)
        {
            updateValues();
            
            if (suffixName == "BURNVECTOR") return GetBurnVector();
            else if (suffixName == "ETA") return UT - Planetarium.GetUniversalTime();
            else if (suffixName == "DELTAV") return GetBurnVector();
            else if (suffixName == "PROGRADE") return Pro;
            else if (suffixName == "RADIALOUT") return RadOut;
            else if (suffixName == "NORMAL") return Norm;
            else if (suffixName == "APOAPSIS")
            {
                if (nodeRef == null) throw new kOSException("Node must be added to flight plan first");
                return nodeRef.nextPatch.ApA;
            }
            else if (suffixName == "PERIAPSIS")
            {
                if (nodeRef == null) throw new kOSException("Node must be added to flight plan first");
                return nodeRef.nextPatch.PeA;
            }

            return base.GetSuffix(suffixName);
        }

        public override bool SetSuffix(string suffixName, object value)
        {
            if (suffixName == "BURNVECTOR" || suffixName == "ETA" || suffixName == "DELTAV") throw new kOSReadOnlyException(suffixName);

            else if (suffixName == "PROGRADE") { Pro = (double)value; UpdateAll(); return true; }
            else if (suffixName == "RADIALOUT") { RadOut = (double)value; UpdateAll(); return true; }
            else if (suffixName == "NORMAL") { Norm = (double)value; UpdateAll(); return true; }
            else if (suffixName == "TIME") { UT = (double)value + Planetarium.GetUniversalTime(); UpdateAll(); return true; }

            return false;
        }

        public void Remove()
        {
            if (nodeRef != null)
            {
                NodeLookup.Remove(nodeRef);

                vesselRef.patchedConicSolver.RemoveManeuverNode(nodeRef);

                nodeRef = null;
                vesselRef = null;
            }
        }

        public override string ToString()
        {
            return "NODE(" + UT + "," + RadOut + "," + Norm + "," + Pro + ")";
        }
    }
}
