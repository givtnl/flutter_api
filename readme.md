# Giving Assistant API
## Contents of this readme
<ul>
<li>Introduction</li>
<li>DynamoDB Access Patterns</li>
<ul>
</ul>
</ul>


###Introduction
//todo

###DynamoDB Access Patterns

* <u>Retrieve all Questions</u>
**Use case**: Application needs to be able retrieve the questions to present them to the user for answering.
*Query Operators*
  + PK = QUESTION
  + SK = STARTSWITH('METADATA')

* <u>Retrieve Tags from a question</u>
**Use case**: Upon answering a question by the user, the system needs to be able retrieve the attached tags from the question, so it can match the user with the tags.
*Query Operators*
    + PK = QUESTION
    + SK = STARTSWITH('{id-of-the-question}#TAG')

* <u>Retrieve match summary from an user</u>
**Use case**: Application needs to be able to build a profile to see how the user matches against a certain set of tags
*Query Operators*
    + PK = USER#{id-of-the-user}
    + SK = STARTSWITH('METADATA#Scores')

* <u>Retrieve individual matches from an user</u>
**Use case**: Application needs to be able to either remove or add or modify the current matches in order to recalculate them. This might occur after an event ( like answering a question ).
*Query Operators*
    + PK = USER#{id-of-the-user}
    + SK = STARTSWITH('MATCH#TAG#')
*Info*
    + Upon modifying one of the individual 'user match tag' rows, the metadata data section Scores needs to be recalculated aswell, this is to prevent the system from executing any database calls when the user requests its matches with certain tags without any state change. It is cheaper to request the metadata than having to re-fetch all the tags and matches everytime

* <u>Retrieve organisation(s) based on a tag</u>
**Use case**: Application needs to be able to match an user with an organisation. Every question has certain tags attached to them. In the system, we also attach these tags to the user ( see previous ) and then we try to find the organisations with the same tags for perform matching
*Query Operators*
    + PK = ORGANISATION
    + SK = STARTSWITH('MATCH#TAG#{name-of-the-tag}')

* <u>Retrieve the profile of an organisation</u>
**Use case**: Application needs to be able to retrieve a detail from an organisation to see the organisation's mission & vision and other interesting details
*Query Operators*
    + PK = ORGANISATION#{id-of-the-organisation}
    + SK = STARTSWITH('METADATA#PROFILE') 