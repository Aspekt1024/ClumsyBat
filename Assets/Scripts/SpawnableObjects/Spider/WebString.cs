using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebString {
    
    private Transform webAnchor;
    private Transform spiderTf;
    private HingeJoint2D spiderHinge;
    private const int numSections = 30;
    private int activeSections;
    private float zLayer;
    private bool isSwinging;

    private const float sectionSize = 0.3f;

    WebSection[] sections;
    Transform[] links;

    private struct WebSection
    {
        public Transform tf;
        public Rigidbody2D body;
        public HingeJoint2D hinge;
    }

	
    public WebString(Transform parentTf)
    {
        spiderTf = parentTf;
        spiderHinge = spiderTf.GetComponent<HingeJoint2D>();
        GenerateSections();
        zLayer = Toolbox.Instance.ZLayers["Spider"] + 0.01f;
    }

    public void UpdateWebSprites()
    {
        if (!isSwinging) return;

        for (int i = 0; i < activeSections - 1; i++)
        {
            PositionLink(i, sections[i].tf.position, sections[i + 1].tf.position);
        }
        PositionLink(activeSections - 1, sections[activeSections - 1].tf.position, spiderTf.position);
        PositionLink(activeSections, sections[0].tf.position, webAnchor.position);
    }

    private void PositionLink(int i, Vector2 node1, Vector2 node2)
    {
        float dist = Vector2.Distance(node1, node2);
        Vector2 midPoint = (node1 + node2) / 2f;

        float angle = -Mathf.Atan((node2.x - node1.x) / (node2.y - node1.y)) * 180f / Mathf.PI;
        if (float.IsNaN(angle)) angle = 0;

        links[i].eulerAngles = new Vector3(0f, 0f, angle);
        links[i].position = new Vector3(midPoint.x, midPoint.y, zLayer + 0.04f);
        links[i].localScale = new Vector2(0.5f, dist / sectionSize);
    }

    public void UpdateDrop()
    {
        int requiredSections = Mathf.FloorToInt(Vector2.Distance(webAnchor.position, spiderTf.position) / sectionSize) - 1;
        while (requiredSections > activeSections && activeSections < numSections)
        {
            AttachSection(activeSections);
        }
    }

    public void Engage()
    {
        spiderHinge.enabled = true;
        spiderTf.GetComponent<Rigidbody2D>().AddForce(new Vector2(200f, Random.Range(-1000, 1000f)));
    }

    public void Disengage()
    {
        spiderHinge.enabled = false;
    }

    public void MoveLeft(float time, float speed)
    {
        if (webAnchor.position.x > 20 || webAnchor.position.x < -20) return;
        webAnchor.position += Vector3.left * time * speed;
    }

    public void Activate(bool IsSwinging, Vector2 anchorPoint)
    {
        foreach (var section in sections)
        {
            section.body.isKinematic = true;
            section.body.transform.position = Toolbox.Instance.HoldingArea;
        }

        isSwinging = IsSwinging;
        if (IsSwinging)
        {
            SetupSwingingWeb(anchorPoint);
        }
        else
        {
            SetFirstSection();
        }
    }

    private void GenerateSections()
    {
        sections = new WebSection[numSections];
        links = new Transform[numSections];

        Transform websParent = GetWebsGroupObject();
        webAnchor = new GameObject("WebAnchor", typeof(Rigidbody2D)).transform;
        webAnchor.GetComponent<Rigidbody2D>().isKinematic = true;
        webAnchor.SetParent(websParent);
        webAnchor.position = Toolbox.Instance.HoldingArea;
        
        Transform linksParent = new GameObject("WebLinks").transform;
        linksParent.SetParent(websParent);

        for (int i = 0; i < numSections; i++)
        {
            GameObject newSection = new GameObject("WebPiece", typeof(Rigidbody2D), typeof(HingeJoint2D));
            newSection.transform.SetParent(webAnchor);
            newSection.transform.position = Toolbox.Instance.HoldingArea;

            sections[i].body = newSection.GetComponent<Rigidbody2D>();
            sections[i].hinge = newSection.GetComponent<HingeJoint2D>();
            sections[i].tf = newSection.transform;

            sections[i].body.isKinematic = true;

            GameObject newLink = new GameObject("WebLink", typeof(SpriteRenderer));
            newLink.transform.SetParent(linksParent);
            newLink.transform.position = Toolbox.Instance.HoldingArea;
            newLink.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Obstacles/Spider/WebPiece");
            links[i] = newLink.transform;
        }
    }

    private void SetupSwingingWeb(Vector2 anchorPoint)
    {
        webAnchor.position = new Vector2(spiderTf.position.x + anchorPoint.x, spiderTf.position.y + anchorPoint.y);
        float anchorDist = Mathf.Sqrt(Vector2.SqrMagnitude(anchorPoint));
        int requiredSections = Mathf.FloorToInt(anchorDist / sectionSize);

        activeSections = 0;
        while (activeSections < requiredSections)
        {
            int sNum = activeSections;

            float posRatio = activeSections / requiredSections;
            Vector3 sectionPos = spiderTf.position + new Vector3(anchorPoint.x, anchorPoint.y, 0.01f);
            sectionPos -= new Vector3(anchorPoint.x, anchorPoint.y, 0f) * posRatio;
            sections[sNum].tf.position = sectionPos;
            
            if (sNum == 0)
            {
                sections[sNum].hinge.connectedBody = webAnchor.GetComponent<Rigidbody2D>();
            }
            else
            {
                sections[sNum].hinge.connectedBody = sections[sNum - 1].body;
            }

            sections[sNum].hinge.autoConfigureConnectedAnchor = false;
            sections[sNum].hinge.anchor = new Vector2(0f, sectionSize / 2);
            sections[sNum].hinge.connectedAnchor = new Vector2(0f, -sectionSize / 2);
            sections[sNum].body.isKinematic = false;
            sections[sNum].body.mass = 5;

            activeSections++;
        }

        spiderHinge.connectedBody = sections[activeSections - 1].body;
        spiderHinge.anchor = new Vector2(0f, .7f);
    }

    private void SetFirstSection()
    {
        activeSections = 1;
        webAnchor.position = spiderTf.transform.position + Vector3.up * 1f;  // TODO use raycast
        sections[0].body.transform.position = new Vector3(webAnchor.position.x, webAnchor.position.y -.5f, zLayer);

        sections[0].hinge.connectedBody = webAnchor.GetComponent<Rigidbody2D>();
        sections[0].hinge.anchor = new Vector2(0f, sectionSize / 2);
        sections[0].hinge.connectedAnchor = sections[0].body.transform.position + Vector3.up * (sectionSize / 2 + 0.06f);

        sections[0].body.isKinematic = false;
    }

    private void AttachSection(int sNum)
    {
        activeSections++;

        sections[sNum].hinge.connectedBody = sections[sNum - 1].body;
        sections[sNum].hinge.anchor = new Vector2(0f, sectionSize / 2);
        sections[sNum].hinge.autoConfigureConnectedAnchor = false;
        sections[sNum].hinge.connectedAnchor = new Vector2(0f, -sectionSize / 2);

        sections[sNum].hinge.useLimits = true;
        JointAngleLimits2D limits = sections[sNum].hinge.limits;
        limits.min = -5f;
        limits.max = 5f;
        sections[sNum].hinge.limits = limits;

        sections[sNum].body.transform.position = new Vector3(sections[sNum - 1].body.transform.position.x, sections[sNum - 1].body.transform.position.y + sectionSize, zLayer);
        sections[sNum].body.isKinematic = false;

        spiderHinge.connectedBody = sections[sNum].body;
        spiderHinge.anchor = new Vector2(0f, 0.2f);
        spiderHinge.autoConfigureConnectedAnchor = false;
        spiderHinge.connectedAnchor = new Vector2(0f, -sectionSize / 2);
    }

    private Transform GetWebsGroupObject()
    {
        Transform spiderParent = GameObject.Find("Spiders").transform;
        Transform webParent = null;

        foreach (Transform t in spiderParent)
        {
            if (t.name == "Webs")
            {
                webParent = t;
                break;
            }
        }

        if (webParent == null)
        {
            webParent = new GameObject("Webs").transform;
            webParent.SetParent(spiderParent);
        }

        return webParent;
    }
}
