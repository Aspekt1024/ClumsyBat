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

    private const float sectionSize = 0.3f;

    WebSection[] sections;

    private struct WebSection
    {
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

    public void Update()
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

    public void Disable()
    {
        Disengage();
        for (int i = 0; i < activeSections; i++)
        {
            sections[i].body.isKinematic = true;
            sections[i].body.velocity = Vector2.zero;
            sections[i].body.angularVelocity = 0;
        }
    }

    public void MoveLeft(float time, float speed)
    {
        if (webAnchor.position.x < 20 && webAnchor.position.x > -20)
            webAnchor.position += Vector3.left * time * speed;
    }

    public void Activate()
    {
        SetFirstSection();
    }

    private void GenerateSections()
    {
        sections = new WebSection[numSections];
        Transform websParent = GetWebsGroupObject();
        webAnchor = new GameObject("WebAnchor").transform;
        webAnchor.SetParent(websParent);
        webAnchor.position = Toolbox.Instance.HoldingArea;

        for (int i = 0; i < numSections; i++)
        {
            GameObject newSection = new GameObject("WebPiece", typeof(Rigidbody2D), typeof(HingeJoint2D), typeof(SpriteRenderer));
            newSection.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Obstacles/Spider/WebPiece");
            newSection.transform.SetParent(webAnchor);
            newSection.transform.position = Toolbox.Instance.HoldingArea;

            sections[i].body = newSection.GetComponent<Rigidbody2D>();
            sections[i].hinge = newSection.GetComponent<HingeJoint2D>();

            sections[i].body.isKinematic = true;
        }
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

        JointAngleLimits2D limits = sections[sNum].hinge.limits;
        limits.min = -10f;
        limits.max = 10f;
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
